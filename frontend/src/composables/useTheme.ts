import { ref, onMounted } from 'vue';

type Theme = 'light' | 'dark';

export function useTheme() {
    const theme = ref<Theme>('light');

    const setTheme = (newTheme: Theme) => {
        theme.value = newTheme;
        localStorage.setItem('theme', newTheme);
        document.documentElement.setAttribute('data-theme', newTheme);
    };

    const toggleTheme = () => {
        setTheme(theme.value === 'light' ? 'dark' : 'light');
    };

    onMounted(() => {
        const savedTheme = localStorage.getItem('theme') as Theme;
        if (savedTheme) {
            setTheme(savedTheme);
        } else {
            // Check system preference
            const systemDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
            setTheme(systemDark ? 'dark' : 'light');
        }
    });

    return {
        theme,
        toggleTheme,
        setTheme
    };
}
