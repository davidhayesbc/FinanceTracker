<script setup lang="ts">
import { useAccounts } from '../composables/useAccounts';
import Card from 'primevue/card';
import Panel from 'primevue/panel';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Chart from 'primevue/chart';
import { computed, ref } from 'vue';

const { accountSummary, recentTransactions, spendingByCategory, netWorthHistory, loading, error } =
  useAccounts();

const chartData = computed(() => {
  return {
    labels: spendingByCategory.value?.map(category => category.category) || [],
    datasets: [
      {
        data: spendingByCategory.value?.map(category => category.amount) || [],
        backgroundColor: [
          '#42A5F5',
          '#66BB6A',
          '#FFA726',
          '#26C6DA',
          '#7E57C2',
          '#EC407A',
          '#EF5350',
          '#78909C',
          '#29B6F6',
          '#AB47BC',
        ],
      },
    ],
  };
});

// Update chart options specifically for the spending chart
const chartOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'right',
      labels: {
        boxWidth: 20,
        padding: 15,
        font: {
          size: 16,
        },
      },
    },
    title: {
      display: false,
    },
  },
  layout: {
    padding: {
      left: 10,
      right: 10,
      top: 10,
      bottom: 10,
    },
  },
  cutout: '50%', // Adjust the size of the doughnut hole
});

// Net Worth Line Chart Data
const netWorthData = computed(() => {
  if (!netWorthHistory.value) return { labels: [], datasets: [] };

  return {
    labels: netWorthHistory.value.labels,
    datasets: [
      {
        label: 'Net Worth',
        data: netWorthHistory.value.values,
        fill: false,
        borderColor: '#42A5F5',
        tension: 0.4,
      },
    ],
  };
});

const netWorthOptions = ref({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false,
    },
    tooltip: {
      callbacks: {
        label: (context: any) => `$${context.raw.toLocaleString()}`,
      },
    },
  },
  scales: {
    y: {
      ticks: {
        callback: (value: number) => `$${value.toLocaleString()}`,
      },
    },
  },
});
</script>

<template>
  <div class="dashboard">
    <h1>Financial Dashboard</h1>

    <!-- Loading state -->
    <div v-if="loading" class="loading-container">
      <i class="pi pi-spin pi-spinner" style="font-size: 2rem"></i>
      <p>Loading your financial data...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="error-container">
      <i class="pi pi-exclamation-triangle" style="font-size: 2rem; color: var(--red-500)"></i>
      <p>{{ error.message || 'An error occurred while loading your data.' }}</p>
    </div>

    <div v-else>
      <!-- Overview Cards -->
      <div class="grid">
        <div class="col-12 md:col-6 lg:col-3">
          <Card class="overview-card">
            <template #title>Total Balance</template>
            <template #content>
              <div class="card-value positive" v-if="accountSummary">
                ${{ accountSummary.totalBalance.toLocaleString() }}
              </div>
            </template>
          </Card>
        </div>
        <div class="col-12 md:col-6 lg:col-3">
          <Card class="overview-card">
            <template #title>Monthly Income</template>
            <template #content>
              <div class="card-value positive">$3,200.00</div>
            </template>
          </Card>
        </div>
        <div class="col-12 md:col-6 lg:col-3">
          <Card class="overview-card">
            <template #title>Monthly Expenses</template>
            <template #content>
              <div class="card-value negative">$1,850.75</div>
            </template>
          </Card>
        </div>
        <div class="col-12 md:col-6 lg:col-3">
          <Card class="overview-card">
            <template #title>Savings Rate</template>
            <template #content>
              <div class="card-value">42%</div>
            </template>
          </Card>
        </div>
      </div>

      <!-- Net Worth Chart -->
      <div class="grid mt-3">
        <div class="col-12">
          <Panel header="Net Worth Growth (Last 12 Months)">
            <div class="chart-container">
              <Chart type="line" :data="netWorthData" :options="netWorthOptions" />
            </div>
          </Panel>
        </div>
      </div>

      <!-- Account Summary & Spending Section -->
      <div class="grid mt-3">
        <div class="col-12 lg:col-6">
          <Panel header="Account Summary">
            <DataTable :value="accountSummary?.accounts" stripedRows>
              <Column field="name" header="Account"></Column>
              <Column field="type" header="Type"></Column>
              <Column field="balance" header="Balance">
                <template #body="slotProps">
                  ${{ slotProps.data.balance.toLocaleString() }}
                </template>
              </Column>
              <Column>
                <template #body>
                  <Button icon="pi pi-eye" class="p-button-text p-button-sm" />
                </template>
              </Column>
            </DataTable>
          </Panel>
        </div>

        <!-- Spending by Category -->
        <div class="col-12 lg:col-6">
          <Panel header="Spending by Category" class="panel-equal-height">
            <div class="chart-container spending-chart">
              <Chart type="doughnut" :data="chartData" :options="chartOptions" />
            </div>
          </Panel>
        </div>
      </div>

      <!-- Recent Transactions -->
      <div class="grid mt-3">
        <div class="col-12">
          <Panel header="Recent Transactions">
            <DataTable :value="recentTransactions" stripedRows>
              <Column field="date" header="Date"></Column>
              <Column field="description" header="Description"></Column>
              <Column field="category" header="Category"></Column>
              <Column field="amount" header="Amount">
                <template #body="slotProps">
                  <span :class="slotProps.data.amount >= 0 ? 'positive' : 'negative'">
                    ${{ Math.abs(slotProps.data.amount).toLocaleString() }}
                  </span>
                </template>
              </Column>
            </DataTable>
            <div class="flex justify-content-end mt-3">
              <Button label="View All Transactions" icon="pi pi-arrow-right" iconPos="right" />
            </div>
          </Panel>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dashboard {
  width: 100%;
  max-width: 100%;
  padding: 0; /* Remove padding */
  box-sizing: border-box;
}
.dashboard h1 {
  margin-bottom: 1.5rem;
  color: var(--text-color);
}

.overview-card {
  height: 100%;
}

.card-value {
  font-size: 1.5rem;
  font-weight: bold;
}

.positive {
  color: #4caf50;
}

.negative {
  color: #f44336;
}

/* Improved chart container */
.chart-container {
  height: 300px;
  width: 100%;
  position: relative;
}

/* Enhance spending chart styling */
.spending-chart {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 350px; /* Increase the height */
  margin: 0 auto;
  max-width: 90%; /* Ensure some margin */
}

/* Equal height panels */
.panel-equal-height {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.panel-equal-height .p-panel-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

/* Ensure the chart fills its container */
.p-chart {
  width: 100% !important;
  height: 100% !important;
  display: flex;
  justify-content: center;
  align-items: center;
}

/* Target PrimeVue Panel contents specifically */
:deep(.p-panel-content) {
  display: flex;
  flex-direction: column;
  flex: 1;
  height: 100%;
}

/* Override any max-heights that might be limiting the container */
:deep(.p-datatable),
:deep(.p-chart) {
  max-height: none !important;
}

.loading-container,
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  text-align: center;
}
</style>
