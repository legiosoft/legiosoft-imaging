# Multi-stage Dockerfile for building libwebp libraries for Linux platforms

# Stage 1: Source - Clone libwebp once
FROM alpine:latest AS source

# Install git and ca-certificates for secure HTTPS cloning
RUN apk add --no-cache git ca-certificates

WORKDIR /source

# Clone libwebp source
ARG WEBP_VERSION=v1.6.0
RUN git clone --depth=1 --branch ${WEBP_VERSION} https://chromium.googlesource.com/webm/libwebp .

# Stage 2: Build for x86_64 (amd64) - using glibc for broad Linux compatibility
FROM ubuntu:22.04 AS builder-amd64

# Install build dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    build-essential \
    cmake \
    make \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# Copy source from source stage
COPY --from=source /source .

# Build shared library
RUN mkdir build && cd build && \
    cmake .. \
        -DBUILD_SHARED_LIBS=ON \
        -DCMAKE_BUILD_TYPE=Release \
        -DCMAKE_POSITION_INDEPENDENT_CODE=ON && \
    make -j$(nproc) && \
    ls -la *.so*

# Copy output to artifacts directory
RUN mkdir -p /output/linux-x64 && \
    find build -name "libwebp.so*" -exec cp {} /output/linux-x64/ \; && \
    ls -lh /output/linux-x64/

# Stage 3: Build for ARM64 (aarch64) using cross-compilation
FROM ubuntu:22.04 AS builder-arm64

# Install cross-compiler and build dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    gcc-aarch64-linux-gnu \
    g++-aarch64-linux-gnu \
    build-essential \
    cmake \
    make \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /build

# Copy source from source stage
COPY --from=source /source .

# Create output directory and build shared library for ARM64
RUN mkdir -p /output/linux-arm64 && mkdir build && cd build && \
    cmake .. \
        -DBUILD_SHARED_LIBS=ON \
        -DCMAKE_BUILD_TYPE=Release \
        -DCMAKE_POSITION_INDEPENDENT_CODE=ON \
        -DCMAKE_SYSTEM_NAME=Linux \
        -DCMAKE_SYSTEM_PROCESSOR=aarch64 \
        -DCMAKE_C_COMPILER=aarch64-linux-gnu-gcc \
        -DCMAKE_CXX_COMPILER=aarch64-linux-gnu-g++ \
        -DCMAKE_AR=/usr/bin/aarch64-linux-gnu-gcc-ar \
        -DCMAKE_STRIP=/usr/bin/aarch64-linux-gnu-strip \
        -DCMAKE_FIND_ROOT_PATH=/usr/aarch64-linux-gnu \
        -DCMAKE_FIND_ROOT_PATH_MODE_PROGRAM=NEVER \
        -DCMAKE_FIND_ROOT_PATH_MODE_LIBRARY=ONLY \
        -DCMAKE_FIND_ROOT_PATH_MODE_INCLUDE=ONLY \
        -DCMAKE_SHARED_LINKER_FLAGS="-Wl,--as-needed -Wl,-z,now" && \
    make -j$(nproc) && \
    ls -la *.so* && \
    cp libwebp.so* /output/linux-arm64/ && \
    ls -lh /output/linux-arm64/

# Stage 4: Combine all artifacts
FROM alpine:latest AS final

# Install dependencies
RUN apk add --no-cache file

# Create output directories
RUN mkdir -p /output/linux-x64
RUN mkdir -p /output/linux-arm64

# Copy Linux x64 library
COPY --from=builder-amd64 /output/linux-x64/ /output/linux-x64/

# Copy Linux ARM64 library
COPY --from=builder-arm64 /output/linux-arm64/ /output/linux-arm64/

# Verify library architecture and dependencies
RUN echo "=== Verification ===" && \
    echo "linux-x64:" && \
    file /output/linux-x64/*.so | head -1 && \
    ldd /output/linux-x64/*.so | head -5 && \
    echo "" && \
    echo "linux-arm64:" && \
    file /output/linux-arm64/*.so | head -1 && \
    ls -lh /output/linux-arm64/ && \
    echo "" && \
    echo "=== Libraries built successfully ==="

# Set working directory to output
WORKDIR /output

# Default command
CMD ["sh", "-c", "ls -lhR /output/"]
