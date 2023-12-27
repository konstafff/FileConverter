export const downloadFile = (data: any, fileName: string, fileType: string) => {
  const a = document.createElement("a");

  a.download = fileName;
  a.href = window.URL.createObjectURL(data);
  const clickEvt = new MouseEvent("click", {
    view: window,
    bubbles: true,
    cancelable: true,
  });
  a.dispatchEvent(clickEvt);
  a.remove();
};
