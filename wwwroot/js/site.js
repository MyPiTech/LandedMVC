function detectScreenSize() {
    const width = window.innerWidth;
    const height = window.innerHeight;

    // Check if the screen is small
    const isSmallScreen = width < 600 || height < 400;

    // Return the boolean value
    return isSmallScreen;
}