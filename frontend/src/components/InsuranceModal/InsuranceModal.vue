<template>
  <div v-if="isOpen" class="insurance-modal-overlay" @mousedown.self="close">
    <div class="insurance-modal">
      <div class="modal-header">
        <h2 class="modal-title">{{ isEditing ? 'Edit Insurance' : 'Add Insurance' }}</h2>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-form">
        <div class="form-group">
          <label class="form-label">Provider</label>
          <input
            v-model="form.provider"
            type="text"
            class="form-input"
            required
            placeholder="e.g. AIA, FWD, Rabbit"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Policy Name</label>
          <input
            v-model="form.policyName"
            type="text"
            class="form-input"
            required
            placeholder="e.g. Life Plan, Health"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Premium (฿)</label>
          <input 
            v-model.number="form.premium" 
            type="number" 
            class="form-input" 
            required
            min="0"
            step="0.01"
          />
        </div>

        <div class="form-group">
          <label class="form-label">Due Date</label>
          <input 
            v-model="form.dueDate" 
            type="date" 
            class="form-input" 
            required
          />
        </div>

        <div class="form-group">
          <label class="form-label">Status</label>
          <select v-model="form.status" class="form-select" required>
            <option value="Upcoming">Upcoming</option>
            <option value="Paid">Paid</option>
            <option value="Overdue">Overdue</option>
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
import type { Insurance } from '../../services/financialService';

// ============================================================================
// Props & Emits
// ============================================================================
const props = defineProps<{
  isOpen: boolean;
  editItem?: Insurance | null;
}>();

const emit = defineEmits(['close', 'save']);

// ============================================================================
// Reactive State
// ============================================================================
const isEditing = ref(false);

const form = ref<Insurance>({
  provider: '',
  policyName: '',
  premium: 0,
  dueDate: '',
  status: 'Upcoming'
});

// ============================================================================
// Component Functions
// ============================================================================
const resetForm = () => {
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  const dateString = tomorrow.toISOString().split('T')[0] || '';
  
  form.value = {
    provider: '',
    policyName: '',
    premium: 0,
    dueDate: dateString,
    status: 'Upcoming'
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
const toDateInputValue = (dateStr: string): string => {
  if (!dateStr) return '';
  // Take only the date part from ISO strings like "2024-03-17T00:00:00"
  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return dateStr.split('T')[0] ?? '';
  // Handle Buddhist Era years
  const year = d.getFullYear() > 2500 ? d.getFullYear() - 543 : d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  return `${year}-${mm}-${dd}`;
};

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    if (props.editItem) {
      isEditing.value = true;
      form.value = {
        ...props.editItem,
        dueDate: toDateInputValue(props.editItem.dueDate)
      };
    } else {
      isEditing.value = false;
      resetForm();
    }
  }
});
</script>

<style scoped src="./InsuranceModal.css"></style>
