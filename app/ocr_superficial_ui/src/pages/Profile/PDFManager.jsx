import { useState, useEffect } from "react";

function PDFManager() {
  const [pdfs, setPdfs] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");

  // 🌟 Danh sách PDF mẫu
  useEffect(() => {
    const dummyPDFs = [
      {
        id: "1",
        name: "Báo cáo học kỳ.pdf",
        src: "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf",
      },
      {
        id: "2",
        name: "Đề cương môn học.pdf",
        src: "https://www.orimi.com/pdf-test.pdf",
      },
      {
        id: "3",
        name: "Slide bài giảng.pdf",
        src: "https://unec.edu.az/application/uploads/2014/12/pdf-sample.pdf",
      },
    ];
    setPdfs(dummyPDFs);
  }, []);

  const handleUpload = (e) => {
    const files = Array.from(e.target.files);
    const newPdfs = files.map((file) => ({
      id: URL.createObjectURL(file),
      name: file.name,
      src: URL.createObjectURL(file),
    }));
    setPdfs((prev) => [...prev, ...newPdfs]);
  };

  const handleDelete = (id) => {
    setPdfs(pdfs.filter((pdf) => pdf.id !== id));
  };

  const filteredPdfs = pdfs.filter((pdf) =>
    pdf.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ padding: "20px" }}>
      <h2>Quản lý tài liệu PDF</h2>

      <input type="file" multiple accept=".pdf" onChange={handleUpload} />
      <input
        type="text"
        placeholder="Tìm kiếm tài liệu..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        style={{ marginLeft: "10px" }}
      />

      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fill, minmax(250px, 1fr))",
          gap: "20px",
          marginTop: "20px",
        }}
      >
        {filteredPdfs.map((pdf) => (
          <div
            key={pdf.id}
            style={{
              border: "1px solid #ccc",
              borderRadius: "8px",
              padding: "10px",
              textAlign: "center",
              position: "relative",
              backgroundColor: "#f9f9f9",
            }}
          >
            <embed
              src={pdf.src}
              type="application/pdf"
              width="100%"
              height="200px"
              style={{ borderRadius: "4px" }}
            />
            <p style={{ marginTop: "10px", fontWeight: "bold" }}>{pdf.name}</p>
            <button
              onClick={() => handleDelete(pdf.id)}
              style={{
                position: "absolute",
                top: "5px",
                right: "5px",
                background: "red",
                color: "white",
                border: "none",
                borderRadius: "4px",
                cursor: "pointer",
              }}
            >
              Xoá
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}

export default PDFManager;
