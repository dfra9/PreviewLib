# Testing Preview Setelah Fix

## Status Docker Container
✅ Container `libre-preview` sudah running dengan LibreOffice 25.2.5.2
✅ Volume mapping sudah bekerja: `E:\PreviewLib\...\storage` → `/data`
✅ Test konversi manual berhasil (xlsx → pdf)

## Cara Test:

### 1. Jalankan Backend C#
```bash
cd APIWebPreview\APIWebPreview
dotnet run
```

Backend akan berjalan di: http://localhost:5104

### 2. Jalankan Frontend Next.js
```bash
cd preview-demo
npm run dev
```

Frontend akan berjalan di: http://localhost:3000

### 3. Test Upload File
1. Buka browser: http://localhost:3000
2. Upload file xlsx/docx/pptx/pdf
3. Preview akan muncul sebagai PDF di iframe

## Perubahan yang Dilakukan:

1. **Docker Container**: Menggunakan `linuxserver/libreoffice:latest` (pre-installed)
2. **Command**: Mengubah dari `soffice` ke `libreoffice`
3. **Konversi**: Semua file dikonversi ke PDF (lebih reliable daripada HTML untuk xlsx)
4. **Script**: Update `fix-docker.ps1` untuk setup yang lebih cepat

## Troubleshooting:

Jika preview masih tidak muncul:
1. Cek log di console aplikasi C#
2. Cek apakah file PDF dibuat di folder `storage/previews/{id}/`
3. Test manual: `docker exec libre-preview libreoffice --version`
