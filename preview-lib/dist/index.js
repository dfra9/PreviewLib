// src/MultiPreview.tsx
import { jsx, jsxs } from "react/jsx-runtime";
function MultiPreview({
  src,
  type,
  title,
  height = 720,
  className
}) {
  const viewerType = type === "xlsx" ? "html" : type === "docx" || type === "pptx" ? "pdf" : type;
  return /* @__PURE__ */ jsxs(
    "div",
    {
      className: className ?? "w-full border rounded-xl overflow-hidden",
      style: { border: "1px solid #e5e7eb", borderRadius: 8 },
      children: [
        title && /* @__PURE__ */ jsx(
          "div",
          {
            style: {
              padding: "8px 12px",
              borderBottom: "1px solid #e5e7eb",
              fontWeight: 600
            },
            children: title
          }
        ),
        /* @__PURE__ */ jsx(
          "iframe",
          {
            src,
            title: title ?? `Preview(${viewerType})`,
            style: { width: "100%", height, border: "none" },
            sandbox: "allow-same-origin allow-scripts allow-popups allow-forms"
          }
        )
      ]
    }
  );
}
export {
  MultiPreview
};
//# sourceMappingURL=index.js.map