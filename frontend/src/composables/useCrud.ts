import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { apiClient } from '../services/apiClient'

export function useApi<T>(endpoint: string, options?: {
  key?: string[]
  staleTime?: number
  enabled?: boolean
}) {
  return useQuery({
    queryKey: options?.key || [endpoint],
    queryFn: () => apiClient.get<T>(endpoint),
    staleTime: options?.staleTime ?? 1000 * 60 * 5,
    enabled: options?.enabled ?? true,
  })
}

export function useApiMutation<TData, TVariables>(
  method: 'post' | 'put' | 'delete',
  endpoint: string,
  options?: { invalidateKeys?: string[][] }
) {
  const queryClient = useQueryClient()
  
  return useMutation<TData, Error, TVariables>({
    mutationFn: (variables) => {
      if (method === 'post') return apiClient.post<TData>(endpoint, variables as any)
      if (method === 'put') {
        const { id, ...data } = variables as { id: string } & TData
        return apiClient.put<TData>(`${endpoint}/${id}`, data as any)
      }
      return apiClient.delete(`${endpoint}/${variables}`) as any
    },
    onSuccess: () => {
      options?.invalidateKeys?.forEach(key => 
        queryClient.invalidateQueries({ queryKey: key })
      )
    },
  })
}

export function useCrud<T>(resource: string) {
  const queryClient = useQueryClient()
  const keys = {
    all: [resource],
    lists: () => [...keys.all, 'list'],
    list: (filters?: any) => [...keys.lists(), filters],
    details: () => [...keys.all, 'detail'],
    detail: (id: any) => [...keys.details(), id],
  }

  // GET /{resource} - Fetch all records with optional filters
  const list = (params?: any) => useQuery({
    queryKey: keys.list(params),
    queryFn: () => apiClient.get<T[]>(`/${resource}`),
  })

  // GET /{resource}/:id - Fetch single record by ID
  const detail = (id: any) => useQuery({
    queryKey: keys.detail(id),
    queryFn: () => apiClient.get<T>(`/${resource}/${id}`),
    enabled: !!id,
  })

  // POST /{resource} - Create new record
  const create = useMutation({
    mutationFn: (data: Partial<T>) => apiClient.post<T>(`/${resource}`, data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: keys.all }),
  })

  // PUT /{resource}/:id - Update existing record
  const update = useMutation({
    mutationFn: ({ id, data }: { id: any; data: Partial<T> }) =>
      apiClient.put<T>(`/${resource}/${id}`, data),
    onSuccess: (_, { id }) => {
      queryClient.invalidateQueries({ queryKey: keys.all })
      queryClient.invalidateQueries({ queryKey: keys.detail(id) })
    },
  })

  // DELETE /{resource}/:id - Remove record
  const remove = useMutation({
    mutationFn: (id: any) => apiClient.delete(`/${resource}/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: keys.all }),
  })

  return { list, detail, create, update, remove, keys }
}
