<template>
  <div class="app-expense-summary">
    <div class="expense-layout">
      <!-- Left: Category list panel -->
      <div class="category-list-panel">
        <div class="section-header">
          <h3 class="section-title">Expense by Source</h3>
          <button class="btn-add" @click="openAddModal" title="Add item">
            <span class="mat-icon">add</span>
          </button>
        </div>

        <div class="category-list">
          <!-- Per bank/app -->
          <div
            v-for="(appData, index) in appSummaries"
            :key="index"
            class="category-row"
            :class="{ active: selectedApp === appData.name }"
            @click="selectCategory(appData.name)"
          >
            <div class="category-badge" :class="`badge-${appData.name.toLowerCase()}`">
              {{ appData.name }}
            </div>
            <div class="category-meta">
              <span class="category-count">{{ appData.count }} items</span>
            </div>
            <div class="category-right">
              <span class="category-amount">{{ formatCurrency(appData.total) }}</span>
              <span class="category-pct">{{ appData.percentage.toFixed(1) }}%</span>
            </div>
            <span class="chevron">›</span>
          </div>

          <!-- Subscription row -->
          <div
            v-if="subscriptionTotal > 0"
            class="category-row"
            :class="{ active: selectedApp === 'Subscription' }"
            @click="selectCategory('Subscription')"
          >
            <div class="category-badge badge-subscription">Subscription</div>
            <div class="category-meta">
              <span class="category-count">{{ subscriptions.length }} items</span>
            </div>
            <div class="category-right">
              <span class="category-amount">{{ formatCurrency(subscriptionTotal) }}</span>
              <span class="category-pct">{{ subscriptionPercentage.toFixed(1) }}%</span>
            </div>
            <span class="chevron">›</span>
          </div>

          <!-- Other row -->
          <div
            v-if="otherTotal > 0"
            class="category-row"
            :class="{ active: selectedApp === 'Other' }"
            @click="selectCategory('Other')"
          >
            <div class="category-badge badge-other">Other</div>
            <div class="category-meta">
              <span class="category-count">{{ otherCount }} items</span>
            </div>
            <div class="category-right">
              <span class="category-amount">{{ formatCurrency(otherTotal) }}</span>
              <span class="category-pct">{{ otherPercentage.toFixed(1) }}%</span>
            </div>
            <span class="chevron">›</span>
          </div>
        </div>

        <div class="total-summary">
          <span>Total</span>
          <span class="total-amount">{{ formatCurrency(grandTotal) }}</span>
        </div>
      </div>

      <!-- Right (desktop) / Below (mobile): Detail panel -->
      <div class="category-detail-panel" ref="detailPanelRef" v-if="selectedApp">
        <div class="detail-header">
          <div class="detail-header-left">
            <div class="category-badge" :class="`badge-${selectedApp.toLowerCase()}`">
              {{ selectedApp === 'Other' ? 'Other' : selectedApp }}
            </div>
            <span class="detail-total">{{ formatCurrency(selectedTotal) }}</span>
          </div>
          <button class="btn-close" @click="selectedApp = null" title="ปิด">✕</button>
        </div>

        <div class="detail-items">
          <div
            v-for="(item, idx) in selectedItems"
            :key="idx"
            class="detail-item"
            :class="{ highlighted: item.isHighlighted }"
          >
            <div class="item-content">
              <div>
                <span class="item-name">{{ item.name }}</span>
                <span v-if="(item as any).remark" class="item-remark">{{ (item as any).remark }}</span>
              </div>
              <span class="item-amount">{{ formatCurrency(item.amount) }}</span>
            </div>

            <div class="item-actions" v-if="selectedApp !== 'Subscription'">
              <button
                class="action-btn btn-highlight"
                :class="{ active: item.isHighlighted }"
                @click.stop="toggleHighlight(item as CategoryBreakdown)"
                title="Highlight"
              >
                <span class="mat-icon">{{ item.isHighlighted ? 'bookmark' : 'bookmark_border' }}</span>
              </button>
              <button class="action-btn btn-edit" @click.stop="openEditModal(item as CategoryBreakdown)" title="Edit">
                <span class="mat-icon">edit</span>
              </button>
              <button class="action-btn btn-delete" @click.stop="confirmDelete(item as CategoryBreakdown)" title="Delete">
                <span class="mat-icon">delete</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty state when nothing selected (desktop only) -->
      <div class="detail-empty" v-else>
        <div class="detail-empty-icon-wrap">
          <span class="mat-icon detail-empty-icon">data_table</span>
        </div>
        <p class="detail-empty-title">No Category Selected</p>
        <p class="detail-empty-sub">Select a source from the list to inspect its line items</p>
      </div>
    </div>

    <ExpenseModal
      :is-open="showModal"
      :edit-item="selectedItem"
      @close="closeModal"
      @save="handleSave"
    />
  </div>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { computed, ref, nextTick } from 'vue';
import { formatCurrency } from '../../utils/formatters';
import { financialService, type CategoryBreakdown, type Subscription } from '../../services/financialService';
import ExpenseModal from '../ExpenseModal/ExpenseModal.vue';

// ============================================================================
// Props & Emits
// ============================================================================
interface Props {
  categories: CategoryBreakdown[];
  subscriptions: Subscription[];
}

const props = defineProps<Props>();
const emit = defineEmits(['refresh']);

// ============================================================================
// Reactive State
// ============================================================================
const selectedApp = ref<string | null>(null);
const showModal = ref(false);
const selectedItem = ref<CategoryBreakdown | null>(null);
const detailPanelRef = ref<HTMLElement | null>(null);

// ============================================================================
// Computed Properties
// ============================================================================
const appSummaries = computed(() => {
  const appGroups = new Map<string, { total: number; count: number; items: CategoryBreakdown[] }>();

  props.categories.forEach(cat => {
    if (cat.bankApp && cat.bankApp !== 'Subscription') {
      const existing = appGroups.get(cat.bankApp) || { total: 0, count: 0, items: [] };
      existing.total += cat.amount;
      existing.count += 1;
      existing.items.push(cat);
      appGroups.set(cat.bankApp, existing);
    }
  });

  return Array.from(appGroups.entries()).map(([name, data]) => ({
    name,
    total: data.total,
    count: data.count,
    items: data.items.sort((a, b) => b.amount - a.amount),
    percentage: grandTotal.value > 0 ? (data.total / grandTotal.value) * 100 : 0
  }));
});

const subscriptionTotal = computed(() =>
  props.subscriptions.reduce((sum, sub) => sum + sub.amount, 0)
);

const subscriptionPercentage = computed(() =>
  grandTotal.value > 0 ? (subscriptionTotal.value / grandTotal.value) * 100 : 0
);

const otherItems = computed(() =>
  props.categories.filter(cat => !cat.bankApp).sort((a, b) => b.amount - a.amount)
);

const otherTotal = computed(() =>
  otherItems.value.reduce((sum, item) => sum + item.amount, 0)
);

const otherCount = computed(() => otherItems.value.length);

const grandTotal = computed(() => {
  const categoriesTotal = props.categories.reduce((sum, cat) => sum + cat.amount, 0);
  return categoriesTotal + subscriptionTotal.value;
});

const otherPercentage = computed(() =>
  grandTotal.value > 0 ? (otherTotal.value / grandTotal.value) * 100 : 0
);

// Items shown in the detail panel
const selectedItems = computed(() => {
  if (!selectedApp.value) return [];
  if (selectedApp.value === 'Subscription') return props.subscriptions as any[];
  if (selectedApp.value === 'Other') return otherItems.value;
  const found = appSummaries.value.find(a => a.name === selectedApp.value);
  return found ? found.items : [];
});

// Total amount of the selected category
const selectedTotal = computed(() => {
  if (!selectedApp.value) return 0;
  if (selectedApp.value === 'Subscription') return subscriptionTotal.value;
  if (selectedApp.value === 'Other') return otherTotal.value;
  const found = appSummaries.value.find(a => a.name === selectedApp.value);
  return found ? found.total : 0;
});

// ============================================================================
// Component Functions
// ============================================================================
const isMobile = () => window.innerWidth <= 768;

const selectCategory = (name: string) => {
  selectedApp.value = selectedApp.value === name ? null : name;
  if (selectedApp.value && isMobile()) {
    nextTick(() => {
      detailPanelRef.value?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }
};

const openAddModal = () => {
  selectedItem.value = null;
  showModal.value = true;
};

const openEditModal = (item: CategoryBreakdown) => {
  selectedItem.value = item;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedItem.value = null;
};

const handleSave = async (item: CategoryBreakdown) => {
  try {
    if (selectedItem.value && selectedItem.value.id) {
      await financialService.updateExpense(selectedItem.value.id, item);
    } else {
      await financialService.addExpense(item);
    }
    emit('refresh');
  } catch (error) {
    console.error('Failed to save expense:', error);
    alert('Failed to save. Please try again.');
  }
};

const toggleHighlight = async (item: CategoryBreakdown) => {
  try {
    const updatedItem = { ...item, isHighlighted: !item.isHighlighted };
    if (updatedItem.id) {
      await financialService.updateExpense(updatedItem.id, updatedItem);
      emit('refresh');
    }
  } catch (error) {
    console.error('Failed to toggle highlight:', error);
    alert('Failed to update highlight.');
  }
};

const confirmDelete = async (item: CategoryBreakdown) => {
  if (!confirm(`Delete "${item.name}"?`)) return;
  try {
    if (item.id) {
      await financialService.deleteExpense(item.id);
      emit('refresh');
    }
  } catch (error) {
    console.error('Failed to delete expense:', error);
    alert('Failed to delete item.');
  }
};
</script>

<style scoped src="./AppExpenseSummary.css"></style>
