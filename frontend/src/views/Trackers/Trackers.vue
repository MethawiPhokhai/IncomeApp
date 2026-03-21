<template>
  <div class="trackers-page">
    <div v-if="loading" class="loading-container">
      <div class="spinner"></div>
      <p>Loading...</p>
    </div>

    <div v-else-if="error" class="error-container">
      <p class="error-message">{{ (error as Error).message }}</p>
      <button @click="() => refetch()" class="btn-retry">Retry</button>
    </div>

    <div v-else-if="dashboard" class="trackers-content">
      <div class="page-header">
        <div>
          <span class="page-eyebrow">Financial Management</span>
          <h1 class="page-title">Trackers</h1>
        </div>
      </div>

      <div class="trackers-grid">
        <InsuranceTracker :insurances="dashboard.insurances" @refresh="() => refetch()" />
        <DebtTracker :debts="dashboard.debts" @refresh="() => refetch()" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { type DashboardSummary } from '../../services/financialService'
import { useApi } from '../../composables/useCrud'
import InsuranceTracker from '../../components/InsuranceTracker/InsuranceTracker.vue'
import DebtTracker from '../../components/DebtTracker/DebtTracker.vue'

const router = useRouter()

const { data: dashboard, isLoading: loading, error, refetch } = useApi<DashboardSummary>(
  '/api/financial/dashboard'
)

onMounted(() => {
  const token = localStorage.getItem('token')
  if (!token) router.push('/')
})
</script>

<style scoped src="./Trackers.css"></style>
