const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

// Shared response handler for all HTTP methods
const handleResponse = async (response: Response): Promise<any> => {
  if (!response.ok) {
    if (response.status === 401) {
      // Token expired or invalid, redirect to login
      localStorage.removeItem('token');
      window.location.href = '/';
      throw new Error('Unauthorized');
    }
    throw new Error('API request failed');
  }

  // Only parse JSON if there's content
  const contentType = response.headers.get('content-type');
  if (contentType && contentType.includes('application/json')) {
    return response.json();
  }
  return null;
};

export const apiClient = {
  async get<T>(endpoint: string, signal?: AbortSignal): Promise<T> {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      },
      signal
    });

    return handleResponse(response);
  },

  async post<T>(endpoint: string, body: any, signal?: AbortSignal): Promise<T> {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      },
      body: JSON.stringify(body),
      signal
    });

    return handleResponse(response);
  },

  async put<T>(endpoint: string, body: any, signal?: AbortSignal): Promise<T> {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      },
      body: JSON.stringify(body),
      signal
    });

    return handleResponse(response);
  },

  async delete(endpoint: string, signal?: AbortSignal): Promise<void> {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
      },
      signal
    });

    await handleResponse(response);
  }
};
