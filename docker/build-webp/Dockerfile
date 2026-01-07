# Multi-stage Dockerfile for building libwebp libraries
# This builds for multiple architectures using cross-compilation

# Version v1.6.0

# Stage 1: Base builder image
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

# Stage 2: Build for x86_64 (amd64)
FROM base AS builder-amd64

WORKDIR /build

# Clone libwebp
RUN git -c http.sslVerify=false clone --depth 1 --branch v1.6.0 https://chromium.googlesource.com/webm/libwebp .

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

# Stage 3: Build for ARM64 (aarch64) using QEMU
FROM ubuntu:22.04 AS builder-arm64

# Enable QEMU for cross-compilation
RUN apt-get update && apt-get install -y --no-install-recommends \
    qemu-user-static \
    binfmt-support \
    gcc-aarch64-linux-gnu \
    g++-aarch64-linux-gnu \
    cmake \
    make \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# Clone libwebp
RUN git -c http.sslVerify=false clone --depth 1 --branch v1.6.0 https://chromium.googlesource.com/webm/libwebp .

# Create output directory and build shared library for ARM64
RUN mkdir -p /output/linux-arm64 && mkdir build && cd build && \
    CC=aarch64-linux-gnu-gcc \
    CXX=aarch64-linux-gnu-g++ \
    cmake .. -DBUILD_SHARED_LIBS=ON -DCMAKE_BUILD_TYPE=Release \
      -DCMAKE_SYSTEM_NAME=Linux \
      -DCMAKE_SYSTEM_PROCESSOR=aarch64 \
      -DCMAKE_C_COMPILER=aarch64-linux-gnu-gcc \
      -DCMAKE_CXX_COMPILER=aarch64-linux-gnu-g++ \
      -DCMAKE_AR=/usr/bin/aarch64-linux-gnu-gcc-ar \
      -DCMAKE_RANLIB=/usr/bin/aarch64-linux-gnu-gcc-ranlib && \
    make -j$(nproc) && \
    cp libwebp.so* /output/linux-arm64/ && \
    ls -lh /output/linux-arm64/

# Stage 4: Build for macOS x86_64 (requires special cross-compilation)
# Note: This is simplified - real macOS builds need macOS SDK
# For now, we'll skip macOS and provide download instructions

# Stage 5: Combine all artifacts
FROM alpine:latest AS final

# Install dependencies
RUN apk add --no-cache git curl tar gzip

# Create output directories
RUN mkdir -p /output/linux-x64
RUN mkdir -p /output/linux-arm64
RUN mkdir -p /output/osx-x64
RUN mkdir -p /output/osx-arm64

# Copy Linux x64 library
COPY --from=builder-amd64 /output/linux-x64/ /output/linux-x64/

# Copy Linux ARM64 library
COPY --from=builder-arm64 /output/linux-arm64/ /output/linux-arm64/

# Download macOS libraries from Google
RUN cd /output/osx-x64 && \
    curl -L -o webp.tar.gz "https://storage.googleapis.com/downloads.webmproject.org/releases/webp/libwebp-1.6.0-mac-x86-64.tar.gz" && \
    tar -xzf webp.tar.gz && \
    cp libwebp-1.6.0-mac-10.15/lib/libwebp.dylib libwebp.dylib && \
    rm -rf webp.tar.gz libwebp-1.6.0-mac-10.15

RUN cd /output/osx-arm64 && \
    curl -L -o webp.tar.gz "https://storage.googleapis.com/downloads.webmproject.org/releases/webp/libwebp-1.6.0-mac-arm64.tar.gz" && \
    tar -xzf webp.tar.gz && \
    cp libwebp-1.6.0-mac-arm64/lib/libwebp.dylib libwebp.dylib && \
    rm -rf webp.tar.gz libwebp-1.6.0-mac-arm64

# Add a simple script to verify libraries
RUN echo '#!/bin/sh' > /output/verify.sh && \
    echo 'echo "Verifying libraries..."' >> /output/verify.sh && \
    echo 'ls -lh /output/linux-x64/' >> /output/verify.sh && \
    echo 'ls -lh /output/linux-arm64/' >> /output/verify.sh && \
    echo 'ls -lh /output/osx-x64/' >> /output/verify.sh && \
    echo 'ls -lh /output/osx-arm64/' >> /output/verify.sh && \
    chmod +x /output/verify.sh

# Set working directory to output
WORKDIR /output

# Default command
CMD ["/output/verify.sh"]


