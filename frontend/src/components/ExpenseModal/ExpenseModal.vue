<template>
  <div v-if="isOpen" class="expense-modal-overlay" @mousedown.self="close">
    <div class="expense-modal">
      <div class="modal-header">
        <h2 class="modal-title">{{ isEditing ? 'Edit Expense' : 'Add Expense' }}</h2>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-form">
        <div class="form-group">
          <label class="form-label">Description</label>
          <input
            v-model="form.name"
            type="text"
            class="form-input"
            required
            placeholder="e.g. Food, Transport, Utilities"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Amount (฿)</label>
          <input 
            v-model.number="form.amount" 
            type="number" 
            class="form-input" 
            required
            min="0"
            step="0.01"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Type</label>
          <select v-model="form.type" class="form-select" required>
            <option value="Fixed">Fixed</option>
            <option value="Variable">Variable</option>
            <option value="Health">Health</option>
            <option value="Family">Family</option>
          </select>
        </div>

        <div class="form-group">
          <label class="form-label">Bank / Source</label>
          <select v-model="form.bankApp" class="form-select" required>
            <option value="Kbank">Kbank</option>
            <option value="KTB">KTB</option>
            <option value="Make">Make by Kbank</option>
            <option value="Dime">Dime</option>
            <option value="Office">Office (Salary deduction)</option>
            <option value="Other">Other</option>
          </select>
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
import { ref, watch } from 'vue';
import type { CategoryBreakdown } from '../../services/financialService';

// ============================================================================
// Props & Emits
// ============================================================================
const props = defineProps<{
  isOpen: boolean;
  editItem?: CategoryBreakdown | null;
}>();

const emit = defineEmits(['close', 'save']);

// ============================================================================
// Reactive State
// ============================================================================
const isEditing = ref(false);

const form = ref<CategoryBreakdown>({
  name: '',
  amount: 0,
  type: 'Variable',
  color: '',
  bankApp: 'Kbank'
});

// ============================================================================
// Component Functions
// ============================================================================
const resetForm = () => {
  form.value = {
    name: '',
    amount: 0,
    type: 'Variable',
    color: '',
    bankApp: 'Kbank'
  };
};

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
  if (newVal) {
    if (props.editItem) {
      isEditing.value = true;
      form.value = { ...props.editItem };
    } else {
      isEditing.value = false;
      resetForm();
    }
  }
});
</script>

<style scoped src="./ExpenseModal.css"></style>
