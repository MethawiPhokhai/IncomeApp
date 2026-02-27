<template>
  <button 
    class="theme-toggle" 
    @click="toggleTheme" 
    :title="theme === 'light' ? 'Switch to Dark Mode' : 'Switch to Light Mode'"
  >
    <span v-if="theme === 'light'">🌙</span>
    <span v-else>☀️</span>
  </button>
</template>

<script setup lang="ts">
// ============================================================================
// Imports
// ============================================================================
import { useTheme } from '../../composables/useTheme';

// ============================================================================
// Composables
// ============================================================================
const { theme, toggleTheme } = useTheme();
</script>

<style scoped>
.theme-toggle {
  background: transparent;
  border: none;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 1.5rem;
  margin-right: 0.5rem;
  padding: 0.25rem;
}

.theme-toggle:hover {
  transform: scale(1.1);
}

/* Logic to adjust icon color if background changes in light/dark mode */
/* But since it's in the header which has a consistent style usually... */
</style>
