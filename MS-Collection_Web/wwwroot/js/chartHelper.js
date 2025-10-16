window.renderChart = (canvasId, type, labels, data, label) => {
    const ctx = document.getElementById(canvasId)?.getContext('2d');
    if (!ctx) return;

    new Chart(ctx, {
        type: type,
        data: {
            labels: labels,
            datasets: [{
                label: label,
                data: data,
                backgroundColor: [
                    '#36A2EB', '#FF6384', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'
                ],
                borderColor: 'rgba(0, 0, 0, 0.1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });

    window.exportarPdf = async (titulo, contenido) => {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();

        const fecha = new Date().toLocaleDateString();
        const imgLogo = new Image();
        imgLogo.src = "/Logo.png";

        imgLogo.onload = function () {
            doc.addImage(imgLogo, "PNG", 10, 10, 30, 30);

            doc.setFontSize(18);
            doc.text("MS Joyería y Relojería", 45, 20);
            doc.setFontSize(14);
            doc.text(`Reporte: ${ titulo }`, 45, 30);
            doc.text(`Fecha: ${ fecha }`, 45, 38);

            let y = 60;

            // Encabezado "Detalle"
            doc.setFontSize(12);
            doc.setTextColor(255, 255, 255);
            doc.setFillColor(100, 100, 100);
            doc.rect(10, y, 190, 10, 'F');
            doc.text("Detalle", 15, y + 7);

            y += 14; //  Este espacio evita que el contenido se sobreponga

            // Contenido
            doc.setTextColor(0, 0, 0);
            contenido.forEach(linea => {
                if (y > 280) {
                    doc.addPage();
                    y = 20;
                }
                doc.text(linea, 15, y);
                y += 10;
            });

            doc.save(`${ titulo }.pdf`);
        };
    };

};
