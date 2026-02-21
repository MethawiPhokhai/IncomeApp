<template>
  <div class="chart-container">
    <Doughnut :data="chartData" :options="chartOptions" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Doughnut } from 'vue-chartjs';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';

ChartJS.register(ArcElement, Tooltip, Legend);

interface Props {
  value: number;
  max: number;
  label: string;
  color?: string;
}

const props = withDefaults(defineProps<Props>(), {
  color: '#f59e0b'
});

const chartData = computed(() => ({
  labels: ['ใช้ไปแล้ว', 'เหลือ'],
  datasets: [{
    data: [props.value, Math.max(0, props.max - props.value)],
    backgroundColor: [props.color, '#e5e7eb'],
    borderWidth: 0,
    cutout: '75%'
  }]
}));

const chartOptions = {
  responsive: true,
  maintainAspectRatio: true,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      enabled: true
    }
  }
};
</script>

<style scoped src="./GaugeChart.css"></style>
