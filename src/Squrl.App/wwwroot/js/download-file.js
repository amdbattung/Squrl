window.getLocalDate = () => {
    const date = new Date();

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");

    return `${year}${month}${day}`;
};

window.downloadFileFromStream = async (fileName, streamReference) => {
    const arrayBuffer = await streamReference.arrayBuffer();

    // Use native Save As when supported.
    if (window.showSaveFilePicker) {
        try {
            const handle = await window.showSaveFilePicker({
                suggestedName: fileName,
                types: [{
                    description: "CSV Files",
                    accept: {
                        "text/csv": [".csv"]
                    }
                }]
            });

            const writable = await handle.createWritable();

            await writable.write(arrayBuffer);
            await writable.close();

            return;
        } catch (error) {
            // User cancelled the Save As dialog.
            if (error.name === "AbortError") {
                return;
            }

            throw error;
        }
    }

    // Fallback for browsers without File System Access API.
    const blob = new Blob([arrayBuffer], {
        type: "text/csv;charset=utf-8"
    });

    const url = URL.createObjectURL(blob);

    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;

    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();

    setTimeout(() => URL.revokeObjectURL(url), 1000);
};