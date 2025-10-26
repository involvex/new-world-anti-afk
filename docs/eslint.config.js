module.exports = [
    {
        files: ['**/*.js'],
        languageOptions: {
            ecmaVersion: 2020,
            sourceType: 'module',
            globals: {
                window: 'readonly',
                document: 'readonly',
            },
        },
        rules: {
            'indent': ['error', 4],
            'linebreak-style': ['error', 'windows'],
            'quotes': ['error', 'single'],
            'semi': ['error', 'always'],
            'no-unused-vars': ['warn', { 'args': 'none' }],
            'no-console': 'off',
        },
    },
];