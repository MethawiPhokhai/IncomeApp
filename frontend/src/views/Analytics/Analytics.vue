<template>
  <div class="analytics-page">
    <!-- Header -->
    <header class="analytics-header">
      <div class="header-content">
        <h1 class="analytics-title">Analytics</h1>
        <div class="header-actions">
          <ThemeToggle />
          <button @click="goBack" class="btn-back" title="กลับไปหน้า Dashboard">⬅</button>
        </div>
      </div>
    </header>

    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <div class="spinner"></div>
      <p>กำลังโหลดข้อมูล...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="error-container">
      <p class="error-message">{{ (error as Error).message }}</p>
      <button @click="() => refetch()" class="btn-retry">ลองอีกครั้ง</button>
    </div>

    <!-- Analytics Content -->
    <div v-else-if="dashboard" class="analytics-content">
      <section class="section-charts">
        <div class="charts-grid">
          <!-- APP Expenses Pie Chart -->
          <div class="chart-card">
            <h3 class="chart-title">สรุปค่าใช้จ่ายตาม App</h3>
            <PieChart :data="savingsVsSpendingData" />
          </div>

          <!-- Top 5 Expenses Bar Chart -->
          <div class="chart-card">
            <h3 class="chart-title">ค่าใช้จ่ายสูงสุด 5 อันดับ</h3>
            <BarChart :data="dashboard.charts.topExpenses" />
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { type DashboardSummary } from '../../services/financialService'
import { useApi } from '../../composables/useCrud'

// Components
import PieChart from '../../components/charts/PieChart/PieChart.vue'
import BarChart from '../../components/charts/BarChart/BarChart.vue'
import ThemeToggle from '../../components/ThemeToggle/ThemeToggle.vue'

// ============================================================================
// Composables
// ============================================================================
const router = useRouter()

// ============================================================================
// Query
// ============================================================================
const { data: dashboard, isLoading: loading, error, refetch } = useApi<DashboardSummary>(
  '/api/financial/dashboard'
)

// ============================================================================
// Computed Properties
// ============================================================================
const savingsVsSpendingData = computed(() => {
  if (!dashboard.value) return []
  return dashboard.value.charts.expensesByApp
})

// ============================================================================
// Component Functions
// ============================================================================
const goBack = () => {
  router.push('/dashboard')
}

// ============================================================================
// Lifecycle Hooks
// ============================================================================
onMounted(() => {
  const token = localStorage.getItem('token')
  if (!token) {
    router.push('/')
  }
})
</script>

<style scoped src="./Analytics.css"></style>
