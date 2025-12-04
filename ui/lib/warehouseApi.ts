import { api } from "./api";

export interface Product {
  id: string;
  name: string;
  sku: string;
  price: number;
  currentQuantity: number;
}

export interface ProductCreateDto {
  name: string;
  sku: string;
  price: number;
  initialQuantity?: number;
}

export interface ProductUpdateDto {
  name: string;
  sku: string;
  price: number;
}

export const warehouseApi = {
  async getProducts(): Promise<Product[]> {
    const res = await api.get("/products");
    return res.data;
  },

  async getProduct(id: string): Promise<Product> {
    const res = await api.get(`/products/${id}`);
    return res.data;
  },

  async createProduct(payload: ProductCreateDto): Promise<Product> {
    const res = await api.post("/products/create", payload);
    return res.data;
  },

  async updateProduct(id: string, payload: ProductUpdateDto): Promise<Product> {
    const res = await api.put(`/products/${id}`, payload);
    return res.data;
  },

  async deleteProduct(id: string): Promise<void> {
    await api.delete(`/products/${id}`);
  },

  async addStock(id: string, amount: number) {
    const res = await api.post(`/products/${id}/add-stock`, { amount });
    return res.data;
  },

  async removeStock(id: string, amount: number) {
    const res = await api.post(`/products/${id}/remove-stock`, { amount });
    return res.data;
  },

  async getStockAt(id: string, at: string) {
    const res = await api.get(`/products/${id}/stock-at?at=${encodeURIComponent(at)}`);
    return res.data;
  },
};