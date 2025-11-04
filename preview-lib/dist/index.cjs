"use strict";
var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// src/index.tsx
var index_exports = {};
__export(index_exports, {
  MultiPreview: () => MultiPreview
});
module.exports = __toCommonJS(index_exports);

// src/MultiPreview.tsx
var import_jsx_runtime = require("react/jsx-runtime");
function MultiPreview({
  src,
  type,
  title,
  height = 720,
  className
}) {
  const viewerType = type === "xlsx" ? "html" : type === "docx" || type === "pptx" ? "pdf" : type;
  return /* @__PURE__ */ (0, import_jsx_runtime.jsxs)(
    "div",
    {
      className: className ?? "w-full border rounded-xl overflow-hidden",
      style: { border: "1px solid #e5e7eb", borderRadius: 8 },
      children: [
        title && /* @__PURE__ */ (0, import_jsx_runtime.jsx)(
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
        /* @__PURE__ */ (0, import_jsx_runtime.jsx)(
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
// Annotate the CommonJS export names for ESM import in node:
0 && (module.exports = {
  MultiPreview
});
//# sourceMappingURL=index.cjs.map