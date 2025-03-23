// ...existing code...
import PrimeVue from 'primevue/config';
import 'primevue/resources/themes/saga-blue/theme.css'; // Theme
import 'primevue/resources/primevue.min.css'; // Core CSS
import 'primeicons/primeicons.css'; // Icons
import 'primeflex/primeflex.css'; // Utility CSS

const app = createApp(App);

app.use(PrimeVue);
// ...existing code...
app.mount('#app');
