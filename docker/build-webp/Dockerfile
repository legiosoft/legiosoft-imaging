# Multi-stage Dockerfile for building libwebp libraries
# This builds for multiple architectures using cross-compilation

# Stage 1: Source - Clone libwebp once
FROM alpine:latest AS source

# Install git and ca-certificates for secure HTTPS cloning
RUN apk add --no-cache git ca-certificates

WORKDIR /source

# Clone libwebp source
ARG WEBP_VERSION=v1.6.0
RUN git clone --depth 1 --branch ${WEBP_VERSION} https://chromium.googlesource.com/webm/libwebp .

# Stage 2: Base builder image
FROM alpine:latest AS base

# Install build dependencies
RUN apk add --no-cache \
    git \
    cmake \
    gcc \
    g++ \
    make \
    musl-dev \
    autoconf \
    automake \
    libtool \
    nasm \
    linux-headers \
    python3 \
    curl \
    tar

# Stage 3: Build for x86_64 (amd64)
FROM base AS builder-amd64

WORKDIR /build

# Copy source from source stage
COPY --from=source /source .

# Build shared library
RUN mkdir build && cd build && \
    cmake .. -DBUILD_SHARED_LIBS=ON -DCMAKE_BUILD_TYPE=Release && \
    make -j$(nproc) && \
    make install

# Copy output to artifacts directory
RUN mkdir -p /output/linux-x64 && \
    (cp build/libwebp.so* /output/linux-x64/ 2>/dev/null || \
     find build -name "libwebp.so*" -exec cp {} /output/linux-x64/ \;) && \
    ls -lh /output/linux-x64/

# Stage 4: Build for ARM64 (aarch64) using cross-compilation
FROM ubuntu:22.04 AS builder-arm64

# Install cross-compiler (QEMU not needed for compilation)
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    gcc-aarch64-linux-gnu \
    g++-aarch64-linux-gnu \
    cmake \
    make \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# Copy source from source stage
COPY --from=source /source .

# Create output directory and build shared library for ARM64
RUN mkdir -p /output/linux-arm64 && mkdir build && cd build && \
    CC=aarch64-linux-gnu-gcc \
    CXX=aarch64-linux-gnu-g++ \
    cmake .. -DBUILD_SHARED_LIBS=ON -DCMAKE_BUILD_TYPE=Release \
      -DCMAKE_SYSTEM_NAME=Linux \
      -DCMAKE_SYSTEM_PROCESSOR=aarch64 \
      -DCMAKE_C_COMPILER=aarch64-linux-gnu-gcc \
      -DCMAKE_CXX_COMPILER=aarch64-linux-gnu-g++ && \
    make -j$(nproc) && \
    cp libwebp.so* /output/linux-arm64/ && \
    ls -lh /output/linux-arm64/

# Stage 5: Build for Windows x64 using MinGW-w64
FROM ubuntu:22.04 AS builder-win-x64

RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    gcc-mingw-w64-x86-64 \
    g++-mingw-w64-x86-64 \
    cmake \
    make \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# Copy source from source stage
COPY --from=source /source .

# Build shared library using MinGW-w64 with SIMD enabled
RUN mkdir -p /output/win-x64 && mkdir build && cd build && \
    CC=x86_64-w64-mingw32-gcc \
    CXX=x86_64-w64-mingw32-g++ \
    cmake .. -DBUILD_SHARED_LIBS=ON -DCMAKE_BUILD_TYPE=Release \
      -DCMAKE_SYSTEM_NAME=Windows \
      -DCMAKE_SYSTEM_PROCESSOR=x86_64 \
      -DCMAKE_C_COMPILER=x86_64-w64-mingw32-gcc \
      -DCMAKE_CXX_COMPILER=x86_64-w64-mingw32-g++ \
      -DWEBP_DISABLE_WIC=ON \
      -DCMAKE_SHARED_LINKER_FLAGS="-static-libgcc -static-libstdc++" && \
    make -j$(nproc) && \
    cp libwebp*.dll /output/win-x64/ 2>/dev/null || \
    find . -name "*.dll" -exec cp {} /output/win-x64/ \; && \
    ls -lh /output/win-x64/

# Stage 6: Combine all artifacts and download macOS libraries
FROM alpine:latest AS final

# Install dependencies
RUN apk add --no-cache curl tar gzip

# Create output directories
RUN mkdir -p /output/linux-x64
RUN mkdir -p /output/linux-arm64
RUN mkdir -p /output/win-x64
RUN mkdir -p /output/osx-x64
RUN mkdir -p /output/osx-arm64

# Copy Linux x64 library
COPY --from=builder-amd64 /output/linux-x64/ /output/linux-x64/

# Copy Linux ARM64 library
COPY --from=builder-arm64 /output/linux-arm64/ /output/linux-arm64/

# Copy Windows x64 library
COPY --from=builder-win-x64 /output/win-x64/ /output/win-x64/

# Download macOS libraries from Google with robust path handling
ARG WEBP_VERSION=v1.6.0
RUN version=${WEBP_VERSION#v} && \
    cd /output/osx-x64 && \
    curl -L -o webp.tar.gz "https://storage.googleapis.com/downloads.webmproject.org/releases/webp/libwebp-${version}-mac-x86-64.tar.gz" && \
    tar -xzf webp.tar.gz && \
    cp libwebp-*/lib/libwebp.dylib libwebp.dylib && \
    rm -rf webp.tar.gz libwebp-*

RUN version=${WEBP_VERSION#v} && \
    cd /output/osx-arm64 && \
    curl -L -o webp.tar.gz "https://storage.googleapis.com/downloads.webmproject.org/releases/webp/libwebp-${version}-mac-arm64.tar.gz" && \
    tar -xzf webp.tar.gz && \
    cp libwebp-*/lib/libwebp.dylib libwebp.dylib && \
    rm -rf webp.tar.gz libwebp-*

# Add a simple script to verify libraries
RUN echo '#!/bin/sh' > /output/verify.sh && \
    echo 'echo "Verifying libraries..."' >> /output/verify.sh && \
    echo 'ls -lh /output/linux-x64/' >> /output/verify.sh && \
    echo 'ls -lh /output/linux-arm64/' >> /output/verify.sh && \
    echo 'ls -lh /output/win-x64/' >> /output/verify.sh && \
    echo 'ls -lh /output/osx-x64/' >> /output/verify.sh && \
    echo 'ls -lh /output/osx-arm64/' >> /output/verify.sh && \
    chmod +x /output/verify.sh

# Set working directory to output
WORKDIR /output

# Default command
CMD ["/output/verify.sh"]
