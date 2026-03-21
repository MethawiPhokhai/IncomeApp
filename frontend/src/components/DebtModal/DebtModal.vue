<template>
  <div v-if="isOpen" class="debt-modal-overlay" @mousedown.self="close">
    <div class="debt-modal">
      <div class="modal-header">
        <h2 class="modal-title">{{ isEditing ? 'Edit Installment' : 'Add Installment' }}</h2>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-form">
        <div class="form-group">
          <label class="form-label">Description</label>
          <input
            v-model="form.name"
            type="text"
            class="form-input"
            required
            placeholder="e.g. iPhone 15 Pro, Lawyer fee"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Monthly Payment (฿)</label>
          <input 
            v-model.number="form.monthlyPayment" 
            type="number" 
            class="form-input" 
            required
            min="0"
            step="0.01"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Current Installment</label>
            <input 
              v-model.number="form.currentInstallment" 
              type="number" 
              class="form-input" 
              required
              min="0"
            />
          </div>

          <div class="form-group">
            <label class="form-label">Total Installments</label>
            <input 
              v-model.number="form.totalInstallments" 
              type="number" 
              class="form-input" 
              required
              min="1"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label class="form-label">Total Amount (฿)</label>
            <div class="form-input form-input--readonly">{{ totalAmount.toLocaleString() }}</div>
          </div>

          <div class="form-group">
            <label class="form-label">Remaining Amount (฿)</label>
            <div class="form-input form-input--readonly">{{ remainingAmount.toLocaleString() }}</div>
          </div>
        </div>

        <div class="modal-actions">
          <button type="button" class="btn btn-secondary" @click="close">Cancel</button>
          <button type="submit" class="btn btn-primary">Save</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { ref, computed, watch } from 'vue';
import type { Debt } from '../../services/financialService';

// ============================================================================
// Props & Emits
// ============================================================================
const props = defineProps<{
  isOpen: boolean;
  editItem?: Debt | null;
}>();

const emit = defineEmits(['close', 'save']);

// ============================================================================
// Reactive State
// ============================================================================
const isEditing = ref(false);

const form = ref<Omit<Debt, 'remainingAmount' | 'totalAmount'>>({
  name: '',
  monthlyPayment: 0,
  currentInstallment: 0,
  totalInstallments: 1,
});

const totalAmount = computed(() => form.value.monthlyPayment * form.value.totalInstallments);
const remainingAmount = computed(() => totalAmount.value - form.value.monthlyPayment * form.value.currentInstallment);

// ============================================================================
// Component Functions
// ============================================================================
const resetForm = () => {
  form.value = {
    name: '',
    monthlyPayment: 0,
    currentInstallment: 0,
    totalInstallments: 1,
  };
};

const close = () => {
  emit('close');
};

const handleSubmit = () => {
  emit('save', { ...form.value, totalAmount: totalAmount.value, remainingAmount: remainingAmount.value });
  close();
};

// ============================================================================
// Watchers
// ============================================================================
watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    if (props.editItem) {
      isEditing.value = true;
      const { remainingAmount: _r, totalAmount: _t, ...rest } = props.editItem;
      form.value = rest;
    } else {
      isEditing.value = false;
      resetForm();
    }
  }
});
</script>

<style scoped src="./DebtModal.css"></style>
