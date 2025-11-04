import * as react_jsx_runtime from 'react/jsx-runtime';

type MultiPreviewProps = {
    src: string;
    type: "pdf" | "html" | "docx" | "pptx" | "xlsx";
    title?: string;
    height?: number;
    className?: string;
};
declare function MultiPreview({ src, type, title, height, className, }: MultiPreviewProps): react_jsx_runtime.JSX.Element;

export { MultiPreview, type MultiPreviewProps };
