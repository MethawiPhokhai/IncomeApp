<template>
  <button
    class="theme-toggle"
    @click="toggleTheme"
    :title="theme === 'light' ? 'Switch to Dark Mode' : 'Switch to Light Mode'"
  >
    <span class="toggle-icon">{{ theme === 'light' ? 'dark_mode' : 'light_mode' }}</span>
  </button>
</template>

<script setup lang="ts">
import { useTheme } from '../../composables/useTheme';

const { theme, toggleTheme } = useTheme();
</script>

<style scoped>
.theme-toggle {
  background: none;
  border: 1px solid #DEE3EB;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  padding: 0.375rem;
  color: #43474E;
  transition: color 0.2s, background 0.2s, border-color 0.2s;
}

.theme-toggle:hover {
  color: #1A237E;
  background: #F1F4F8;
  border-color: #1A237E;
}

[data-theme="dark"] .theme-toggle {
  color: #ffffff;
  border-color: #1e3050;
}

[data-theme="dark"] .theme-toggle:hover {
  color: #ffffff;
  background: #1e2f47;
  border-color: #4a7abf;
}

.toggle-icon {
  font-family: 'Material Symbols Outlined', sans-serif;
  font-size: 1.25rem;
  font-weight: 400;
  font-style: normal;
  line-height: 1;
  letter-spacing: normal;
  text-transform: none;
  white-space: nowrap;
  font-variation-settings: 'FILL' 0, 'wght' 300, 'GRAD' 0, 'opsz' 24;
}
</style>
