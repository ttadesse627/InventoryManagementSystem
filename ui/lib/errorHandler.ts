import toast from "react-hot-toast";


export function handleError(error: any): void {
  


    // Helper functions for different error types
    const handleAxiosError = (err: any) => {
    if (err.code === 'ECONNABORTED') {
        toast.error("Request timeout. Please try again.");
    } else if (err.code === 'NETWORK_ERROR') {
        toast.error("Network error. Check your connection.");
    } else if (err.response) {
        handleHttpError(err.response);
    } else {
        toast.error("An unexpected error occurred.");
    }
    };

    const handleHttpError = (response: any) => {
    const status = response.status;
    
    switch (status) {
        case 400:
        toast.error("Bad request. Please check your input.");
        break;
        case 401:
        toast.error("Authentication required. Please login again.");
        // Optional: Redirect to login
        // router.push('/login');
        break;
        case 403:
        toast.error("You don't have permission to access this resource.");
        break;
        case 404:
        toast.error("Requested resource not found.");
        break;
        case 429:
        toast.error("Too many requests. Please try again later.");
        break;
        case 503:
        toast.error("Warehouse service is temporarily unavailable.");
        break;
        case 500:
        case 502:
        case 504:
        toast.error("Server error. Please try again later.");
        break;
        default:
        toast.error(`Error ${status}: Failed to load products.`);
    }
    };

    const handleNetworkError = (err: TypeError) => {
    if (err.message.includes('Failed to fetch') || 
        err.message.includes('Network request failed')) {
        toast.error("Network error. Please check your internet connection.");
    } else {
        toast.error("Connection failed. Please try again.");
    }
    };

    const handleNoResponseError = (err: any) => {
    console.error('No response received:', err);
    toast.error("No response from server. The service might be down.");
    };

    if (error.isAxiosError) { // If using Axios
        handleAxiosError(error);
    } else if (error instanceof TypeError) { // Network errors
        handleNetworkError(error);
    } else if (error.response) { // HTTP response errors
        handleHttpError(error.response);
    } else if (error.request) { // Request made but no response
        handleNoResponseError(error);
    } else {
        // Generic error
        toast.error("Failed to load products.");
    }
}