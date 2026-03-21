<template>
  <div class="insurance-tracker">
    <div class="section-header">
      <h3 class="section-title">Incoming Payment</h3>
      <button class="btn-add" @click="openAddModal" title="Add insurance">
        <span class="mat-icon">add</span>
      </button>
    </div>

    <div class="timeline">
      <div
        v-for="(insurance, index) in sortedInsurances"
        :key="insurance.id"
        class="timeline-item"
        :class="getDotStatus(insurance.dueDate)"
      >
        <!-- Dot + vertical line -->
        <div class="timeline-track">
          <div class="timeline-dot"></div>
          <div v-if="index < sortedInsurances.length - 1" class="timeline-line"></div>
        </div>

        <!-- Content -->
        <div class="timeline-body">
          <div class="timeline-date">{{ formatTimelineDate(insurance.dueDate) }}</div>
          <div class="timeline-row">
            <div class="timeline-info">
              <div class="timeline-name">{{ insurance.provider }}</div>
              <div class="timeline-meta">
                <span class="timeline-amount">{{ formatCurrency(insurance.premium) }}</span>
                <span class="meta-sep">·</span>
                <span class="timeline-policy">{{ insurance.policyName || '—' }}</span>
              </div>
            </div>
            <div class="item-actions">
              <button class="action-btn btn-edit" @click.stop="openEditModal(insurance)" title="Edit">
                <span class="mat-icon">edit</span>
              </button>
              <button class="action-btn btn-delete" @click.stop="confirmDelete(insurance)" title="Delete">
                <span class="mat-icon">delete</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer summary -->
    <div class="schedule-footer">
      <div class="footer-left">
        <span class="footer-label">NEXT 30 DAYS</span>
        <span class="footer-amount">{{ formatCurrency(next30DaysTotal) }}</span>
      </div>
      <span class="mat-icon footer-icon">calendar_month</span>
    </div>

    <InsuranceModal
      :is-open="showModal"
      :edit-item="selectedItem"
      @close="closeModal"
      @save="handleSave"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { formatCurrency, getDaysUntil } from '../../utils/formatters';
import { financialService, type Insurance } from '../../services/financialService';
import InsuranceModal from '../InsuranceModal/InsuranceModal.vue';

interface Props {
  insurances: Insurance[];
}

const props = defineProps<Props>();
const emit = defineEmits(['refresh']);

const showModal = ref(false);
const selectedItem = ref<Insurance | null>(null);

// ── Computed ──────────────────────────────────────────────────────
const sortedInsurances = computed(() =>
  [...props.insurances].sort(
    (a, b) => getDaysUntil(a.dueDate) - getDaysUntil(b.dueDate)
  )
);

const next30DaysTotal = computed(() => {
  return props.insurances
    .filter(i => {
      const d = getDaysUntil(i.dueDate);
      return d >= 0 && d <= 30;
    })
    .reduce((sum, i) => sum + i.premium, 0);
});

// ── Helpers ───────────────────────────────────────────────────────
const formatTimelineDate = (dateStr: string): string => {
  const d = new Date(dateStr);
  if (isNaN(d.getTime())) return dateStr;
  const year = d.getFullYear() > 2500 ? d.getFullYear() - 543 : d.getFullYear();
  const monthDay = d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }).toUpperCase();
  return `${monthDay}, ${year}`;
};

const getDotStatus = (dueDate: string): string => {
  const days = getDaysUntil(dueDate);
  if (days < 0) return 'dot-overdue';
  if (days <= 7) return 'dot-soon';
  return 'dot-future';
};

// ── Actions ───────────────────────────────────────────────────────
const openAddModal = () => {
  selectedItem.value = null;
  showModal.value = true;
};

const openEditModal = (item: Insurance) => {
  selectedItem.value = item;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedItem.value = null;
};

const handleSave = async (item: Insurance) => {
  try {
    if (selectedItem.value?.id) {
      await financialService.updateInsurance(selectedItem.value.id, item);
    } else {
      await financialService.addInsurance(item);
    }
    emit('refresh');
  } catch {
    alert('Failed to save. Please try again.');
  }
};

const confirmDelete = async (item: Insurance) => {
  if (!confirm(`Delete "${item.provider} - ${item.policyName}"?`)) return;
  try {
    if (item.id) {
      await financialService.deleteInsurance(item.id);
      emit('refresh');
    }
  } catch {
    alert('Failed to delete.');
  }
};
</script>

<style scoped src="./InsuranceTracker.css"></style>
