<template>
  <div v-if="isOpen" class="summary-modal-overlay" @mousedown.self="close">
    <div class="summary-modal">
      <div class="modal-header">
        <h2 class="modal-title">แก้ไขข้อมูลสรุป</h2>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-form">
        <div class="form-group">
          <label class="form-label">รายได้รวม (Income)</label>
          <input 
            v-model.number="form.income" 
            type="number" 
            class="form-input" 
            required
            min="0"
          />
        </div>

        <div class="form-group">
          <label class="form-label">เงินออมรวม (Total Savings)</label>
          <input 
            v-model.number="form.totalSavings" 
            type="number" 
            class="form-input" 
            required
            min="0"
          />
        </div>

        <div class="form-group">
          <label class="form-label">เงินลงทุนรวม (Total Investment)</label>
          <input 
            v-model.number="form.totalInvestment" 
            type="number" 
            class="form-input" 
            required
            min="0"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Net Worth Growth (Monthly)</label>
          <input 
            v-model.number="form.netWorthGrowth" 
            type="number" 
            class="form-input" 
            required
          />
          <span class="form-node">ค่าใช้จ่ายรวม (Expenses) จะคำนวณอัตโนมัติตามรายการที่บันทึก</span>
        </div>

        <div class="modal-actions">
          <button type="button" class="btn btn-secondary" @click="close">ยกเลิก</button>
          <button type="submit" class="btn btn-primary">บันทึก</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { ref, watch } from 'vue';
import type { DashboardSummary, UpdateSummaryRequest } from '../../services/financialService';

// ============================================================================
// Props & Emits
// ============================================================================
const props = defineProps<{
  isOpen: boolean;
  currentData?: DashboardSummary;
}>();

const emit = defineEmits(['close', 'save']);

// ============================================================================
// Reactive State
// ============================================================================
const form = ref<UpdateSummaryRequest>({
  income: 0,
  totalSavings: 0,
  totalInvestment: 0,
  netWorthGrowth: 0
});

// ============================================================================
// Component Functions
// ============================================================================
const close = () => {
  emit('close');
};

const handleSubmit = () => {
  emit('save', { ...form.value });
  close();
};

// ============================================================================
// Watchers
// ============================================================================
watch(() => props.isOpen, (newVal) => {
  if (newVal && props.currentData) {
    form.value = {
      income: props.currentData.income,
      totalSavings: props.currentData.totalSavings,
      totalInvestment: props.currentData.totalInvestment,
      netWorthGrowth: props.currentData.netWorthGrowth
    };
  }
});
</script>

<style scoped src="./SummaryEditModal.css"></style>
