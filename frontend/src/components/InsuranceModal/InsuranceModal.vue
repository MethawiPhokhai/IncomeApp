<template>
  <div v-if="isOpen" class="insurance-modal-overlay" @mousedown.self="close">
    <div class="insurance-modal">
      <div class="modal-header">
        <h2 class="modal-title">{{ isEditing ? 'แก้ไขประกัน' : 'เพิ่มประกันใหม่' }}</h2>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-form">
        <div class="form-group">
          <label class="form-label">บริษัทประกัน</label>
          <input 
            v-model="form.provider" 
            type="text" 
            class="form-input" 
            required
            placeholder="เช่น AIA, FWD, Rabbit"
          />
        </div>

        <div class="form-group">
          <label class="form-label">ชื่อกรมธรรม์</label>
          <input 
            v-model="form.policyName" 
            type="text" 
            class="form-input" 
            required
            placeholder="เช่น แผนชีวิต, สุขภาพ"
          />
        </div>

        <div class="form-group">
          <label class="form-label">เบี้ยประกัน (บาท)</label>
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
          <label class="form-label">วันครบกำหนด</label>
          <input 
            v-model="form.dueDate" 
            type="date" 
            class="form-input" 
            required
          />
        </div>

        <div class="form-group">
          <label class="form-label">สถานะ</label>
          <select v-model="form.status" class="form-select" required>
            <option value="Upcoming">กำลังจะถึง (Upcoming)</option>
            <option value="Paid">จ่ายแล้ว (Paid)</option>
            <option value="Overdue">เกินกำหนด (Overdue)</option>
          </select>
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

<style scoped src="./InsuranceModal.css"></style>
