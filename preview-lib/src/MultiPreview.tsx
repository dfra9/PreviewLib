export type MultiPreviewProps = {
  src: string;
  type: "pdf" | "html" | "docx" | "pptx" | "xlsx";
  title?: string;
  height?: number;
  className?: string;
};

export function MultiPreview({
  src,
  type,
  title,
  height = 720,
  className,
}: MultiPreviewProps) {
  const viewerType =
    type === "xlsx"
      ? "html"
      : type === "docx" || type === "pptx"
      ? "pdf"
      : type;

  return (
    <div
      className={className ?? "w-full border rounded-xl overflow-hidden"}
      style={{ border: "1px solid #e5e7eb", borderRadius: 8 }}
    >
      {title && (
        <div
          style={{
            padding: "8px 12px",
            borderBottom: "1px solid #e5e7eb",
            fontWeight: 600,
          }}
        >
          {title}
        </div>
      )}
      <iframe
        src={src}
        title={title ?? `Preview(${viewerType})`}
        style={{ width: "100%", height, border: "none" }}
        sandbox="allow-same-origin allow-scripts allow-popups allow-forms"
      />
    </div>
  );
}
