#!/bin/bash
set -e

echo "=========================================="
echo "  WongaAuth Backend Build Script"
echo "=========================================="

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

print_step() {
    echo -e "${YELLOW}[STEP] $1${NC}"
}

print_success() {
    echo -e "${GREEN}[SUCCESS] $1${NC}"
}

print_error() {
    echo -e "${RED}[ERROR] $1${NC}"
}

# Parse arguments
BUILD_TYPE=${1:-"docker"}  # docker | local | test

case $BUILD_TYPE in
    "docker")
        print_step "Building and starting all services with Docker Compose..."
        docker-compose down --remove-orphans
        docker-compose build --no-cache
        docker-compose up -d
        print_success "All services started!"
        echo ""
        echo "Services running at:"
        echo "  API Gateway:   http://localhost:5000"
        echo "  Auth Service:  http://localhost:5001"
        echo "  User Service:  http://localhost:5002"
        echo "  Auth Swagger:  http://localhost:5001/swagger"
        echo "  User Swagger:  http://localhost:5002/swagger"
        ;;
    "local")
        print_step "Restoring packages..."
        dotnet restore WongaAuth.sln
        print_step "Building solution..."
        dotnet build WongaAuth.sln -c Release
        print_success "Build successful!"
        ;;
    "test")
        print_step "Restoring packages..."
        dotnet restore WongaAuth.sln
        print_step "Running unit tests..."
        dotnet test WongaAuth.sln --verbosity normal --filter "Category!=Integration"
        print_success "All tests passed!"
        ;;
    "test-all")
        print_step "Running all tests (requires running Docker services)..."
        dotnet test WongaAuth.sln --verbosity normal
        print_success "All tests passed!"
        ;;
    *)
        echo "Usage: ./build.sh [docker|local|test|test-all]"
        echo "  docker   - Build and run with Docker Compose (default)"
        echo "  local    - Build locally without Docker"
        echo "  test     - Run unit tests"
        echo "  test-all - Run all tests including integration"
        exit 1
        ;;
esac
