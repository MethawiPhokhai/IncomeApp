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
      <p class="error-message">{{ error }}</p>
      <button @click="loadDashboard" class="btn-retry">ลองอีกครั้ง</button>
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
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import { useRouter } from 'vue-router';
import { financialService, type DashboardSummary } from '../../services/financialService';
import PieChart from '../../components/charts/PieChart/PieChart.vue';
import BarChart from '../../components/charts/BarChart/BarChart.vue';
import ThemeToggle from '../../components/ThemeToggle/ThemeToggle.vue';

const router = useRouter();

const dashboard = ref<DashboardSummary | null>(null);
const loading = ref(true);
const error = ref('');
const abortController = ref<AbortController | null>(null);

const savingsVsSpendingData = computed(() => {
  if (!dashboard.value) return [];
  // Use APP-based expense data directly from backend
  return dashboard.value.charts.expensesByApp;
});

const loadDashboard = async () => {
  if (abortController.value) {
    abortController.value.abort();
  }

  loading.value = true;
  error.value = '';
  abortController.value = new AbortController();
  
  try {
    dashboard.value = await financialService.getDashboard(abortController.value.signal);
  } catch (err: any) {
    if (err.name === 'AbortError') return;
    error.value = 'ไม่สามารถโหลดข้อมูลได้ กรุณาลองใหม่อีกครั้ง';
    console.error('Data load error:', err);
  } finally {
    loading.value = false;
  }
};

const goBack = () => {
  router.push('/dashboard');
};

onMounted(() => {
  const token = localStorage.getItem('token');
  if (!token) {
    router.push('/');
    return;
  }
  loadDashboard();
});

onBeforeUnmount(() => {
  if (abortController.value) {
    abortController.value.abort();
  }
});
</script>

<style scoped src="./Analytics.css"></style>
