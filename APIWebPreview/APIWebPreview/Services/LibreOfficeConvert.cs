using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace APIWebPreview.Services
{
    public class LibreOfficeConvert
    {
        private readonly LibreOfficeOptions _opt;

        public LibreOfficeConvert(IOptions<LibreOfficeOptions> opt)
        {
            _opt = opt.Value;
        }

        public bool CanRun()
        {
            if (_opt.UseDocker)
            {
                return !string.IsNullOrWhiteSpace(_opt.DockerCliPath)
                       && !string.IsNullOrWhiteSpace(_opt.DockerContainer);
            }

            return !string.IsNullOrWhiteSpace(_opt.SofficePath)
                   && File.Exists(_opt.SofficePath);
        }

        private string ToContainerPath(string hostPath)
        {
            // contoh hostPath: E:\PreviewLib\APIWebPreview\APIWebPreview\storage\uploads\abc.xlsx
            var p = hostPath.Replace("\\", "/");
            
            // Cari posisi "storage" di path
            var storageIdx = p.LastIndexOf("/storage/", StringComparison.OrdinalIgnoreCase);
            if (storageIdx < 0)
            {
                storageIdx = p.LastIndexOf("storage/", StringComparison.OrdinalIgnoreCase);
                if (storageIdx >= 0) storageIdx -= 1; // adjust untuk "/"
            }
            
            if (storageIdx >= 0)
            {
                var tail = p.Substring(storageIdx + "/storage".Length); // /uploads/abc.xlsx
                return _opt.DockerWorkDir.TrimEnd('/') + tail;
            }
            
            return _opt.DockerWorkDir.TrimEnd('/') + "/" + Path.GetFileName(p);
        }

        private ProcessStartInfo BuildDockerPsi(string inputHost, string outputHost, string extraArgs)
        {
            Directory.CreateDirectory(outputHost);

            var inputContainer = ToContainerPath(inputHost);
            var outputContainer = ToContainerPath(outputHost);

            var args =
                $"exec {_opt.DockerContainer} libreoffice --headless --norestore --nolockcheck --nodefault {extraArgs} --outdir \"{outputContainer}\" \"{inputContainer}\"";

            return new ProcessStartInfo
            {
                FileName = _opt.DockerCliPath,
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        private ProcessStartInfo BuildWindowsPsi(string inputPath, string outputDir, string extraArgs)
        {
            Directory.CreateDirectory(outputDir);

            var soPath = _opt.SofficePath!;
            var workDir = Path.GetDirectoryName(soPath) ?? Directory.GetCurrentDirectory();

            return new ProcessStartInfo
            {
                FileName = soPath,
                Arguments = $"--headless --norestore --nolockcheck --nodefault {extraArgs} --outdir \"{outputDir}\" \"{inputPath}\"",
                WorkingDirectory = workDir,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        public async Task<string> ConvertToPdfAsync(string inputPath, string outputDir, CancellationToken ct = default)
        {
            ProcessStartInfo psi = _opt.UseDocker
                ? BuildDockerPsi(inputPath, outputDir, "--convert-to pdf")
                : BuildWindowsPsi(inputPath, outputDir, "--convert-to pdf");

            Console.WriteLine($"[LibreOffice] Converting to PDF: {inputPath}");
            Console.WriteLine($"[LibreOffice] Output dir: {outputDir}");
            Console.WriteLine($"[LibreOffice] Command: {psi.FileName} {psi.Arguments}");

            using var p = Process.Start(psi)!;
            await p.WaitForExitAsync(ct);

            var stdout = await p.StandardOutput.ReadToEndAsync();
            var stderr = await p.StandardError.ReadToEndAsync();

            Console.WriteLine($"[LibreOffice] Exit code: {p.ExitCode}");
            if (!string.IsNullOrWhiteSpace(stdout)) Console.WriteLine($"[LibreOffice] STDOUT: {stdout}");
            if (!string.IsNullOrWhiteSpace(stderr)) Console.WriteLine($"[LibreOffice] STDERR: {stderr}");

            if (p.ExitCode != 0)
            {
                throw new Exception($"LibreOffice PDF convert failed (exit {p.ExitCode}): {stderr}");
            }

            var pdfName = Path.GetFileNameWithoutExtension(inputPath) + ".pdf";
            var resultPath = Path.Combine(outputDir, pdfName);
            
            if (!File.Exists(resultPath))
            {
                throw new Exception($"PDF file not created at expected path: {resultPath}");
            }

            return resultPath;
        }

        public async Task<string> ConvertXlsxToHtmlAsync(string inputPath, string outputDir, CancellationToken ct = default)
        {
            ProcessStartInfo psi = _opt.UseDocker
                ? BuildDockerPsi(inputPath, outputDir, "--convert-to html:\"calc_html_Export\"")
                : BuildWindowsPsi(inputPath, outputDir, "--convert-to html:\"calc_html_Export\"");

            Console.WriteLine($"[LibreOffice] Converting to HTML: {inputPath}");
            Console.WriteLine($"[LibreOffice] Output dir: {outputDir}");
            Console.WriteLine($"[LibreOffice] Command: {psi.FileName} {psi.Arguments}");

            using var p = Process.Start(psi)!;
            await p.WaitForExitAsync(ct);

            var stdout = await p.StandardOutput.ReadToEndAsync();
            var stderr = await p.StandardError.ReadToEndAsync();

            Console.WriteLine($"[LibreOffice] Exit code: {p.ExitCode}");
            if (!string.IsNullOrWhiteSpace(stdout)) Console.WriteLine($"[LibreOffice] STDOUT: {stdout}");
            if (!string.IsNullOrWhiteSpace(stderr)) Console.WriteLine($"[LibreOffice] STDERR: {stderr}");

            if (p.ExitCode != 0)
            {
                throw new Exception($"LibreOffice HTML convert failed (exit {p.ExitCode}): {stderr}");
            }

            var htmlName = Path.GetFileNameWithoutExtension(inputPath) + ".html";
            var resultPath = Path.Combine(outputDir, htmlName);
            
            if (!File.Exists(resultPath))
            {
                throw new Exception($"HTML file not created at expected path: {resultPath}");
            }

            return resultPath;
        }
    }
}
