import axios from "axios";

//Set the api base URL from environment variable
export const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL + "/api",
});
