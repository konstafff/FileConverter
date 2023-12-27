import * as Icon from "react-bootstrap-icons";
import { Button } from "react-bootstrap";
import { useAppDispatch, useAppSelector } from "../../app-store/hooks";
import { filesActions } from "../../app-store/slices/files-slice";
import { v4 as uuidv4 } from "uuid";
import { ChangeEvent, useEffect, useRef, useState } from "react";
import Alert from "react-bootstrap/Alert";

import styles from "./style.module.scss";
import { uploadFile } from "../../api-services/files-api-service";

export const UploadFileComponent = () => {
  const appDispatch = useAppDispatch();
  const files = useAppSelector((x) => x.files.items);
  const authKey = useAppSelector((x) => x.auth.key);
  const [errorMessage, setErrorMessage] = useState<string | undefined>();

  useEffect(() => {
    if (!errorMessage) return;

    setTimeout(() => {
      setErrorMessage(undefined);
    }, 3000);
  }, [errorMessage]);

  const inputFile = useRef<HTMLInputElement>(null);

  const handleAddFile = () => {
    inputFile?.current?.click();
  };

  const handleFileChange = (e: ChangeEvent<HTMLInputElement>) => {
    if (
      !e.target.files ||
      !e.target.files.length ||
      e.target.files[0].size === 0
    )
      return;

    const selectedFile = e.target.files[0];

    //It can be better
    if (
      files.some(
        (x) =>
          x.fileName.trim() === selectedFile.name.trim() &&
          x.fileSize === selectedFile.size
      )
    ) {
      e.target.value = "";

      setErrorMessage("This file has already been converted");
      return;
    }

    const file = {
      fileId: uuidv4(),
      fileName: selectedFile.name,
      fileSize: selectedFile.size,
      isConverted: false,
      isError: false,
    };

    appDispatch(filesActions.add(file));

    e.target.value = "";

    uploadFile(selectedFile, file, authKey).catch(() => {
      appDispatch(filesActions.setError(file.fileId));
    });
  };
  return (
    <div className="d-flex justify-content-center">
      <Button variant="success" onClick={handleAddFile}>
        <Icon.Upload /> Upload HTML files
      </Button>
      <input
        type="file"
        id="file"
        ref={inputFile}
        style={{ display: "none" }}
        accept=".html"
        onChange={handleFileChange}
      />
      {errorMessage && (
        <Alert variant="danger" className={styles.alert}>
          {errorMessage}
        </Alert>
      )}
    </div>
  );
};
