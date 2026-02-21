<template>
  <div class="chart-container">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { computed } from 'vue';
import { Bar } from 'vue-chartjs';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Tooltip, Legend } from 'chart.js';
import type { ChartDataPoint } from '../../../services/financialService';

// ============================================================================
// Chart.js Registration
// ============================================================================
ChartJS.register(CategoryScale, LinearScale, BarElement, Tooltip, Legend);

// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  data: ChartDataPoint[];
  title?: string;
}

const props = defineProps<Props>();

// ============================================================================
// Computed Properties
// ============================================================================
const chartData = computed(() => ({
  labels: props.data.map(d => d.label),
  datasets: [{
    label: 'ค่าใช้จ่าย',
    data: props.data.map(d => d.value),
    backgroundColor: props.data.map(d => d.color),
    borderRadius: 8,
    barThickness: 40
  }]
}));

// ============================================================================
// Chart Configuration
// ============================================================================
const chartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      callbacks: {
        label: function(context: any) {
          const value = context.parsed.y || 0;
          return `฿${value.toLocaleString()}`;
        }
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        callback: function(value: any) {
          return '฿' + value.toLocaleString();
        }
      }
    }
  }
};
</script>

<style scoped src="./BarChart.css"></style>
