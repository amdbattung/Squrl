window.getLocalDate = () => {
    const date = new Date();

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");

    return `${year}${month}${day}`;
};

window.downloadFileFromStream = async (fileName, streamReference) => {
    const fileTypes = {
        ".txt": {
            description: "Text Files",
            pickerMimeType: "text/plain",
            blobMimeType: "text/plain;charset=utf-8"
        },
        ".csv": {
            description: "CSV Files",
            pickerMimeType: "text/csv",
            blobMimeType: "text/csv;charset=utf-8"
        },
        ".xlsx": {
            description: "Excel Files",
            pickerMimeType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            blobMimeType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        },
        ".xls": {
            description: "Excel 97-2003 Files",
            pickerMimeType: "application/vnd.ms-excel",
            blobMimeType: "application/vnd.ms-excel"
        }
    };

    const extension = Object.keys(fileTypes)
        .find(ext => fileName.toLowerCase().endsWith(ext));

    const fileType = extension
        ? fileTypes[extension]
        : {
            description: "Files",
            mimeType: "application/octet-stream"
        };
    
    const arrayBuffer = await streamReference.arrayBuffer();

    // Use native Save As when supported.
    if (window.showSaveFilePicker) {
        try {
            const handle = await window.showSaveFilePicker({
                suggestedName: fileName,
                types: [{
                    description: fileType.description,
                    accept: {
                        [fileType.pickerMimeType]: [extension]
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
        type: fileType.blobMimeType
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