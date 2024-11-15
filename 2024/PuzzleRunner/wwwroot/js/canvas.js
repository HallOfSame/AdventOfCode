window.AocCanvas = {
    draw: function (canvasId, imageData) {
        const canvas = document.getElementById(canvasId);
        const pixelSize = imageData.pixelSize;
        const width = imageData.width * pixelSize;
        const height = imageData.height * pixelSize;
    
        const ctx = canvas.getContext("2d");
        ctx.canvas.width = width;
        ctx.canvas.height = height;
        ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
        ctx.font = `${pixelSize}px square`;

        function drawText(x, y, text, color) {
            const colorToUse = color || "black";
            if (ctx.fillStyle !== colorToUse) {
                ctx.fillStyle = colorToUse;
            }
            ctx.fillText(text, x * pixelSize, (y + 1) * pixelSize);
        }

        function drawSquare(x, y, color) {
            ctx.fillStyle = color;
            ctx.fillRect(x * pixelSize, y * pixelSize, pixelSize, pixelSize);
        }

        function cartToScreen(px, py) {
            // Convert coordinates to handle 0,0 being at the top right of the canvas
            return {
                x: px,
                y: -py + (height / pixelSize) - 1
            };
        };

        imageData.pixels.forEach((pixel) => {
            const coords = cartToScreen(pixel.x, pixel.y);

            if (pixel.text) {
                drawText(coords.x, coords.y, pixel.text, pixel.color);
            } else {
                drawSquare(coords.x, coords.y, pixel.color);
            }
        });
    },

    enableCoordinateDisplay: function (canvasId, dotnetHelper) {
        const canvas = document.getElementById(canvasId);
        canvas.addEventListener("mousemove", (event) => {
            // Get the mouse position relative to the canvas
            const rect = canvas.getBoundingClientRect();  // Get canvas position relative to the viewport
            const x = event.clientX - rect.left;          // Mouse X position
            const y = event.clientY - rect.top;           // Mouse Y position

            //const translatedX = Math.floor(x / pixelSize);
            //const translatedY = Math.floor(y / pixelSize);

            dotnetHelper.invokeMethodAsync("OnCanvasMouseMove", x, y);
            // Display the coordinates
            // coordsDisplay.textContent = `Mouse Coordinates: (${translatedX.toFixed(2)}, ${translatedY.toFixed(2)})`;
        });
    }
}