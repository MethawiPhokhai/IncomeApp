<template>
  <div class="category-breakdown">
    <h3 class="section-title">{{ title }}</h3>
    <div class="category-list">
      <div 
        v-for="category in categories" 
        :key="category.name"
        class="category-item"
      >
        <div class="category-info">
          <span class="category-dot" :style="{ backgroundColor: category.color }"></span>
          <span class="category-name">{{ category.name }}</span>
        </div>
        <span class="category-amount">{{ formatCurrency(category.amount) }}</span>
      </div>
    </div>
    <div v-if="showTotal" class="category-total">
      <span>รวม</span>
      <span class="total-amount">{{ formatCurrency(total) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { formatCurrency } from '../../utils/formatters';
import type { CategoryBreakdown as CategoryData } from '../../services/financialService';

interface Props {
  title: string;
  categories: CategoryData[];
  showTotal?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  showTotal: true
});

const total = computed(() => 
  props.categories.reduce((sum, cat) => sum + cat.amount, 0)
);
</script>

<style scoped src="./CategoryBreakdown.css"></style>
