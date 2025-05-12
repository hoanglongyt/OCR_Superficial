// src/pages/Loading/Loading.jsx
import { useState, useEffect, useContext } from "react";
import { extractText, greyOut, toImage } from "../../../services/coreService";
import { ImageContext } from "../../../contexts/ImageContext";
import config from "../../../config";
import { useNavigate } from "react-router-dom";

import StepIndicator from "./StepIndicator";
import ImagePreview from "./ImagePreview";
import Checkmark from "./Checkmark";

export default function Loading() {
  const navigate = useNavigate();
  const { image: baseImage, setImage } = useContext(ImageContext);
  const [image, setWorkingImage] = useState(baseImage); // new working copy of image
  const [currentImagePath, setCurrentImagePath] = useState(null);
  const [urlToRevoke, setUrlToRevoke] = useState(null);
  const [imageLoaded, setImageLoaded] = useState(false);
  const [currentStep, setCurrentStep] = useState(null);
  const [isComplete, setIsComplete] = useState(false);
  const [hasError, setHasError] = useState(false);
  const [failedStep, setFailedStep] = useState(undefined);
  const [isRetrying, setIsRetrying] = useState(false);

  const username = "Alex"; // <- You handle dynamically here!

  const steps = [
    {
      stepId: 0,
      stepDescription: "Uploading image",
      handler: async () => {
        const url = URL.createObjectURL(image);
        if (!config.isProduction) console.log(url);
        setCurrentImagePath(url);
        setUrlToRevoke(url);
        setImageLoaded(false);
        await new Promise((resolve) => setTimeout(resolve, 1000));
      },
    },
    {
      stepId: 1,
      stepDescription: "Reprocessing image",
      handler: async () => {
        const imgPath = await toImage(image, setWorkingImage);
        setCurrentImagePath(imgPath);
        setUrlToRevoke(imgPath);
        setImageLoaded(false);
        await new Promise((resolve) => setTimeout(resolve, 1000));
      },
    },
    {
      stepId: 2,
      stepDescription: "Extracting text",
      handler: async () => {
        const greyImg = await greyOut(image, setWorkingImage);
        setCurrentImagePath(greyImg);
        setUrlToRevoke(greyImg);
        setImageLoaded(false);

        const text = await extractText(image);

        await new Promise((resolve) => {
          setTimeout(() => {
            navigate("/process/result", {
              state: { text },
            });
            setImage(null);
            resolve();
          }, 1000);
        });
      },
    },
  ];

  const runSteps = async (startIndex = 0) => {
    try {
      for (let i = startIndex; i < steps.length; i++) {
        const step = steps[i];
        setCurrentStep(step);
        await step.handler();
      }
      setIsComplete(true);
    } catch (error) {
      console.error("Error at step:", currentStep?.stepId, error);
      setFailedStep(currentStep);
      setHasError(true);
    }
  };

  useEffect(() => {
    runSteps();
    return () => {
      if (urlToRevoke) URL.revokeObjectURL(urlToRevoke);
    };
  }, []);

  const handleRetry = async () => {
    setWorkingImage(baseImage);
    setIsComplete(false);
    setHasError(false);
    setIsRetrying(true);
    setCurrentStep(null);
    setImageLoaded(false);
    setCurrentImagePath(null);
    setUrlToRevoke(null);
    await runSteps(failedStep?.stepId ?? 0);
    setIsRetrying(false);
  };

  const retryingStepIndex = Number.isInteger(failedStep?.stepId) ? failedStep.stepId + 1 : 1;

  return (
    <div className="process-container">
      <h2>Hello {username}, we're processing your image...</h2>

      {hasError ? (
        <div className="error-block">
          <div className="error-icon">😳</div>
          <p className="error-text">Oops! Something went wrong during processing.</p>
          <div className="error-buttons">
            <button className="error-button return" onClick={() => navigate("/")}>Go to Home</button>
            <button className="error-button retry" onClick={handleRetry}>Try Again</button>
          </div>
        </div>
      ) : (
        <>
          <p>{isRetrying ? `Retrying from step ${retryingStepIndex}...` : currentStep?.stepDescription}</p>

          <div className="loading-line">
            <div
              className="loader-bar"
              style={{
                width: `${((currentStep?.stepId + (isComplete ? 1 : 0)) / steps.length) * 100}%`,
              }}
            />
          </div>

          <StepIndicator currentStep={currentStep} steps={steps} isComplete={isComplete} />

          <p>
            Processing step {Math.min((currentStep?.stepId ?? 0) + 1, steps.length)} of {steps.length}...
          </p>

          {isComplete ? (
            <Checkmark />
          ) : (
            <ImagePreview
              imagePath={currentImagePath}
              imageLoaded={imageLoaded}
              setImageLoaded={setImageLoaded}
            />
          )}
        </>
      )}
    </div>
  );
}