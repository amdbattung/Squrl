window.productImage = {
    process: async function (inputId, maxWidth, quality) {
        const input = document.getElementById(inputId);

        if (!input) {
            throw new Error(`File input '${inputId}' was not found.`);
        }

        const file = input.files?.[0];

        if (!file) {
            throw new Error("No image selected.");
        }

        const allowedTypes = [
            "image/jpeg",
            "image/png",
            "image/webp"
        ];

        if (!allowedTypes.includes(file.type)) {
            throw new Error(
                "Please select a JPEG, PNG, or WebP image."
            );
        }

        const maxOriginalFileSize = 10 * 1024 * 1024;

        if (file.size > maxOriginalFileSize) {
            throw new Error(
                `The selected image is too large. Maximum size is ${maxOriginalFileSize / (1024 * 1024)} MB.`
            );
        }

        // Decode image.
        const bitmap = await createImageBitmap(file);

        let width = bitmap.width;
        let height = bitmap.height;

        // Resize only if necessary.
        if (width > maxWidth) {
            const scale = maxWidth / width;

            width = Math.round(width * scale);
            height = Math.round(height * scale);
        }

        const canvas = document.createElement("canvas");

        canvas.width = width;
        canvas.height = height;

        const context = canvas.getContext("2d");

        if (!context) {
            bitmap.close();
            throw new Error("Unable to create canvas.");
        }

        context.drawImage(
            bitmap,
            0,
            0,
            width,
            height
        );

        bitmap.close();

        // Encode.
        const blob = await new Promise((resolve, reject) => {
            canvas.toBlob(
                blob => {
                    if (blob) {
                        resolve(blob);
                    } else {
                        reject(
                            new Error("Unable to encode image.")
                        );
                    }
                },
                "image/webp",
                quality
            );
        });
        
        const buffer = await blob.arrayBuffer();

        return {
            bytes: new Uint8Array(buffer),
            mimeType: "image/jpeg"
        };
    }
};