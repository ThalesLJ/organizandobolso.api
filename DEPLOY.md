# 🚀 Deployment Guide

This guide explains how to deploy the API (.NET 8) and the Admin (Blazor) to a VPS using GitHub Actions and Docker.

## 📋 Prerequisites

### 1. Linux VPS
- Ubuntu
- Docker and Docker Compose installed
- User with sudo permissions
- Ports 5000 and 5001 open in the firewall

### 2. GitHub Secrets
Configure the following secrets in your GitHub repository:

| Secret | Description | Example |
|--------|-------------|---------|
| `VPS_SSH_KEY` | Private key (.pem) content | `-----BEGIN RSA PRIVATE KEY-----...` |
| `VPS_HOST` | VPS IP or domain | `203.0.113.10` |
| `VPS_USER` | VPS SSH user | `ubuntu` |
| `MONGODB_URI` | MongoDB Cloud connection string | `mongodb+srv://user:pass@cluster/db` |
| `DOCKER_USERNAME` | Docker Hub username | `thaleslj` |
| `DOCKER_PASSWORD` | Docker Hub access token | `dckr_pat_...` |
| `API_BASE_URL` | Public API URL | `https://api.domain.com` |

## 🔧 VPS Setup

### 1. Install Docker
```bash
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker $USER

sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
```

### 2. Directory structure
```bash
mkdir -p ~/organizandotudo-api
mkdir -p ~/organizandotudo-admin
```

## 🚀 Automatic Deploy

### 1. Push to master
Deploy happens automatically when you push to the `master` branch.

### 2. Manual Deploy
1. Go to the "Actions" tab
2. Select "Build and Deploy to VPS"
3. Click "Run workflow"

## 🔄 How Deploy Works

### 1. Build on GitHub Actions
- Code is built on .NET 8 (API and Admin)
- Two Docker images are created and pushed to Docker Hub (`thaleslj/organizandobolso-api` and `thaleslj/organizandobolso-admin`)
- Automatic tags: `latest`, `master`, `sha-<commit>`

### 2. Deploy on VPS
- A single `docker-compose.yml` is sent to both directories
- In `~/organizandotudo-api` only the `api` service is started
- In `~/organizandotudo-admin` only the `admin` service is started
- MongoDB is consumed via MongoDB Cloud

### 3. Benefits
- Fast build on GitHub Actions
- Fast deploy by pulling ready images
- Lower resource usage on the VPS
- Consistent and versioned process

## 🔄 Updates

To update the application:
1. Make changes to the code
2. Commit and push to `master`
3. GitHub Actions will deploy automatically
4. Containers will be recreated with the new version