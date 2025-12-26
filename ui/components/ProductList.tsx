"use client";

import React, { useEffect, useState } from "react";
import { Product, warehouseApi } from "../lib/warehouseApi";
import StockModal from "./StockModal";
import StockHistory from "./StockHistory";
import ProductForm from "./ProductForm";
import Modal from "./Modal";
import toast from "react-hot-toast";
import { 
  Search, 
  PlusCircle, 
  Package, 
  DollarSign, 
  Hash,
  Edit2,
  Trash2,
  History,
  Plus,
  Minus,
  Loader2
} from "lucide-react";
import { handleError } from "@/lib/errorHandler";

export default function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [mode, setMode] = useState<
    "add" | "remove" | "history" | "create" | "edit" | null
  >(null);
  const [busyId, setBusyId] = useState<string | null>(null);

  const loadProducts = async () => {
  setLoading(true);
  
  try {
    const data = await warehouseApi.getProducts();
    setProducts(data);
    
  } catch (err: any) {
    handleError(err);
  } finally {
    setLoading(false);
  }
};

  useEffect(() => {
    loadProducts();
  }, []);

  const filtered = products.filter(
    (p) =>
      p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.sku.toLowerCase().includes(search.toLowerCase())
  );

  async function handleDelete(p: Product) {
    if (!confirm(`Are you sure you want to delete '${p.name}'?`)) return;

    setBusyId(p.id);
    try {
      await warehouseApi.deleteProduct(p.id);
      setProducts((prev) => prev.filter((x) => x.id !== p.id));
      toast.success("Product deleted successfully");
    } catch (err: any) {
      toast.error(err?.response?.data || "Delete failed");
    } finally {
      setBusyId(null);
    }
  }

  return (
    <div className="min-h-screen bg-linear-to-br from-gray-50 to-gray-100 p-4 md:p-6">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-800 mb-2">Product Inventory</h1>
        <p className="text-gray-600">Manage your warehouse products and stock levels</p>
      </div>

      {/* Top Bar */}
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 mb-6 p-4 bg-white rounded-xl shadow-sm">
        <div className="relative w-full md:w-auto flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-5 w-5" />
          <input
            placeholder="Search by name or SKU..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg w-full focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
          />
        </div>

        <button
          onClick={() => setMode("create")}
          className="flex items-center gap-2 bg-linear-to-r from-blue-600 to-blue-700 text-white px-5 py-2.5 rounded-lg hover:from-blue-700 hover:to-blue-800 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all shadow-md hover:shadow-lg"
        >
          <PlusCircle className="h-5 w-5" />
          <span className="font-medium">New Product</span>
        </button>
      </div>

      {/* Loading State */}
      {loading && (
        <div className="flex flex-col items-center justify-center py-16">
          <Loader2 className="h-12 w-12 text-blue-600 animate-spin mb-4" />
          <p className="text-gray-600">Loading products...</p>
        </div>
      )}

      {/* Products Grid */}
      {!loading && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filtered.map((p) => (
            <div
              key={p.id}
              className="bg-white rounded-xl border border-gray-200 p-5 shadow-sm hover:shadow-md transition-all duration-200"
            >
              {/* Product Header */}
              <div className="flex justify-between items-start mb-4">
                <div className="flex-1">
                  <h3 className="font-bold text-lg text-gray-800 truncate">{p.name}</h3>
                  <div className="flex items-center gap-2 text-sm text-gray-500 mt-1">
                    <Package className="h-4 w-4" />
                    <span className="font-mono">{p.sku}</span>
                  </div>
                </div>
                <div className="px-3 py-1 bg-blue-50 text-blue-700 rounded-full text-sm font-medium">
                  {p.currentQuantity} in stock
                </div>
              </div>

              {/* Product Details */}
              <div className="space-y-3 mb-5">
                <div className="flex items-center gap-2 text-gray-700">
                  <DollarSign className="h-4 w-4 text-green-600" />
                  <span className="font-medium">Price:</span>
                  <span className="text-green-700 font-semibold">
                    ${parseFloat(p.price.toString()).toFixed(2)}
                  </span>
                </div>
                
                {/* Stock Indicator */}
                <div className="w-full bg-gray-200 rounded-full h-2.5">
                  <div 
                    className={`h-2.5 rounded-full ${
                      p.currentQuantity > 20 
                        ? "bg-green-500" 
                        : p.currentQuantity > 10 
                        ? "bg-yellow-500" 
                        : "bg-red-500"
                    }`}
                    style={{ 
                      width: `${Math.min(p.currentQuantity / 50 * 100, 100)}%` 
                    }}
                  ></div>
                </div>
              </div>

              {/* Action Buttons */}
              <div className="flex flex-wrap gap-2">
                <button
                  onClick={() => {
                    setSelectedProduct(p);
                    setMode("add");
                  }}
                  className="flex items-center gap-1.5 px-3 py-2 bg-green-50 text-green-700 hover:bg-green-100 rounded-lg transition-colors flex-1 justify-center"
                >
                  <Plus className="h-4 w-4" />
                  <span className="text-sm font-medium">Add</span>
                </button>

                <button
                  onClick={() => {
                    setSelectedProduct(p);
                    setMode("remove");
                  }}
                  className="flex items-center gap-1.5 px-3 py-2 bg-orange-50 text-orange-700 hover:bg-orange-100 rounded-lg transition-colors flex-1 justify-center"
                >
                  <Minus className="h-4 w-4" />
                  <span className="text-sm font-medium">Remove</span>
                </button>

                <button
                  onClick={() => {
                    setSelectedProduct(p);
                    setMode("history");
                  }}
                  className="flex items-center gap-1.5 px-3 py-2 bg-purple-50 text-purple-700 hover:bg-purple-100 rounded-lg transition-colors"
                  title="View History"
                >
                  <History className="h-4 w-4" />
                </button>

                <button
                  onClick={() => {
                    setSelectedProduct(p);
                    setMode("edit");
                  }}
                  className="flex items-center gap-1.5 px-3 py-2 bg-blue-50 text-blue-700 hover:bg-blue-100 rounded-lg transition-colors"
                  title="Edit"
                >
                  <Edit2 className="h-4 w-4" />
                </button>

                <button
                  onClick={() => handleDelete(p)}
                  disabled={busyId === p.id}
                  className="flex items-center gap-1.5 px-3 py-2 bg-red-50 text-red-700 hover:bg-red-100 disabled:bg-gray-100 disabled:text-gray-400 rounded-lg transition-colors"
                  title="Delete"
                >
                  {busyId === p.id ? (
                    <Loader2 className="h-4 w-4 animate-spin" />
                  ) : (
                    <Trash2 className="h-4 w-4" />
                  )}
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Empty State */}
      {!loading && filtered.length === 0 && (
        <div className="flex flex-col items-center justify-center py-16 bg-white rounded-xl border border-gray-200">
          <Package className="h-16 w-16 text-gray-300 mb-4" />
          <h3 className="text-lg font-semibold text-gray-700 mb-2">No products found</h3>
          <p className="text-gray-500 mb-6">
            {search ? "Try a different search term" : "Get started by creating your first product"}
          </p>
          <button
            onClick={() => setMode("create")}
            className="flex items-center gap-2 bg-blue-600 text-white px-5 py-2.5 rounded-lg hover:bg-blue-700 transition-colors"
          >
            <PlusCircle className="h-5 w-5" />
            Create Product
          </button>
        </div>
      )}

      {/* Modals */}
      <div className="max-h-[calc(90vh-6rem)] overflow-y-auto p-4">
        {mode === "create" && (
          <Modal onClose={() => setMode(null)}>
            <ProductForm
              onCancel={() => setMode(null)}
              onSaved={(prod) => {
                setMode(null);
                setProducts((prev) => [prod, ...prev]);
                toast.success("Product created successfully");
              }}
            />
          </Modal>
        )}

        {selectedProduct && mode === "edit" && (
          <Modal
            onClose={() => {
              setSelectedProduct(null);
              setMode(null);
            }}
          >
            <ProductForm
              initial={selectedProduct}
              onCancel={() => {
                setSelectedProduct(null);
                setMode(null);
              }}
              onSaved={(updated) => {
                setProducts((prev) =>
                  prev.map((x) => (x.id === updated.id ? updated : x))
                );
                setSelectedProduct(null);
                setMode(null);
                toast.success("Product updated successfully");
              }}
            />
          </Modal>
        )}

        {selectedProduct &&
          (mode === "add" || mode === "remove" || mode === "history") && (
            <Modal
              onClose={() => {
                setSelectedProduct(null);
                setMode(null);
              }}
            >
              {mode === "history" ? (

                <StockHistory
                  product={selectedProduct}
                  onClose={() => {
                    setSelectedProduct(null);
                    setMode(null);
                  }}
                />
              ) : (
                
                <StockModal
                  mode={mode as "add" | "remove"}
                  product={selectedProduct}
                  onClose={() => {
                    setSelectedProduct(null);
                    setMode(null);
                    loadProducts();
                  }}
                />
              )}
            </Modal>
          )}
        </div>
    </div>
  );
}