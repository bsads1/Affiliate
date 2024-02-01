module.exports = {
    content: ["Pages/**/*.razor"],
    theme: {
        extend: {},
    },
    daisyui: {
        themes: ["corporate","light"],
    },
    plugins: [require("@tailwindcss/typography"), require("daisyui")],
}

