/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        background: "#F2F2F7",
        "ios-blue": "#007AFF",
        secondary: "#8E8E93",
      },
      borderRadius: {
        "ios-xl": "20px",
      },
    },
  },
  plugins: [],
}
