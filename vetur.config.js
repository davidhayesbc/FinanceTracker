/** @type {import('vls').VeturConfig} */
module.exports = {
  projects: [
    {
      // Root of the Vue project
      root: './FinanceTracker.WebUI',
      // Where the `tsconfig.json` or `jsconfig.json` is located
      tsconfig: './tsconfig.json',
      // Optional: Override Vetur settings for this project
      vetur: {
        useWorkspaceDependencies: true,
        validation: {
          template: true,
          style: true,
          script: true
        }
      }
    }
  ]
};
