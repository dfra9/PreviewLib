#!/bin/bash
# Script untuk fix Docker container LibreOffice

echo "Menghapus container lama..."
docker rm -f libre-preview 2>/dev/null || true

echo "Membuat container baru dengan LibreOffice CLI..."
docker run -d \
  --name libre-preview \
  -v /mnt/e/PreviewLib/APIWebPreview/APIWebPreview/storage:/data \
  ubuntu:22.04 \
  bash -c "apt-get update && apt-get install -y libreoffice fonts-dejavu && tail -f /dev/null"

echo "Menunggu instalasi selesai (30 detik)..."
sleep 30

echo "Cek status container:"
docker ps --filter "name=libre-preview"

echo ""
echo "Test konversi:"
docker exec libre-preview soffice --version
