"use client";
import React from "react";

export default function Modal({
  children,
  onClose,
  size = "md",
}: {
  children: React.ReactNode;
  onClose(): void;
  size?: "sm" | "md" | "lg" | "xl";
}) {
  const sizeClasses = {
    sm: "max-w-md",
    md: "max-w-lg",
    lg: "max-w-2xl",
    xl: "max-w-4xl",
  };

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center p-4"
      onClick={onClose}
    >
      <div className="absolute inset-0 bg-linear-to-br from-black/40 to-black/60 backdrop-blur-sm border-2" />

      {/* Modal */}
      <div
        className={`relative w-full ${sizeClasses[size]} animate-scaleIn max-h-[calc(90vh-2rem)] no-scrollbar overflow-y-auto p-4`}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="bg-white rounded-xl shadow-2xl overflow-hidden border border-gray-200">
          {/* Optional: Header with close button */}
          <div className="flex justify-end p-2">
            <button
              onClick={onClose}
              className="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
              aria-label="Close"
            >
              <svg
                className="w-5 h-5"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </button>
          </div>

          {/* Contents */}
          <div className="px-6 pb-6 pt-0">
            {children}
          </div>
        </div>
      </div>
    </div>
  );
}