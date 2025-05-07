# JavaScript, TypeScript, and Vue.js Styling Guide

This document outlines the styling and coding conventions for JavaScript, TypeScript, and Vue.js development within the FinanceTracker project. Consistency with these guidelines is expected to maintain code quality and readability.

## General JavaScript/TypeScript Standards

- **Indentation:** Use 2 spaces for indentation.
- **Strings:**
  - Use single quotes (`'`) for string literals.
  - Use template literals (`` `Hello ${name}` ``) for dynamic strings or multi-line strings.
- **`const` vs. `let`:**
  - Use `const` for variables whose values will not be reassigned.
  - Use `let` for variables that will be reassigned.
- **Arrow Functions:** Use arrow functions for callbacks and component methods where appropriate, especially within the Composition API in Vue.
- **Destructuring:** Use object and array destructuring where it improves code readability and conciseness.
- **ES6+ Features:** Utilize modern ECMAScript features supported by the project's target browsers and Node.js version (as configured in Vite/TypeScript).
- **Comments:** Use JSDoc/TSDoc style comments for functions, methods, and complex props.
- **Async/Await:** Utilize `async/await` for all I/O-bound and genuinely asynchronous operations.

## TypeScript Specifics

- **Strong Typing:** Leverage TypeScript for strong typing. Define interfaces or types for component props, events, API payloads, and store state.
- **Strict Mode:** Enable and adhere to strict type checking options in `tsconfig.json`.

## Vue.js Specifics

- **Naming Conventions:**
  - Components: `PascalCase` (e.g., `TransactionTable.vue`, `EditAccountModal.vue`).
  - Variables, Functions, Props, Data Properties, Computed Properties: `camelCase`.
- **API Style:** Prefer Vue 3 Composition API with `<script setup>` for new components.
- **Event Emission:** Emit events using `kebab-case` (e.g., `defineEmits(['update:modelValue', 'form-submitted'])`).
- **Props:** Props should be clearly defined with types, and `required` or `default` values where appropriate.
- **State Management:** (Conventions for Pinia, Vuex, or simple composables to be detailed here by the user if applicable).
- **API Interaction:** (Conventions for the chosen library like Axios or native fetch, including service structure, request/response handling, and error handling, to be detailed here by the user).

---

_This guide should be updated as project standards evolve._
