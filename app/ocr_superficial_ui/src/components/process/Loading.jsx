import { useState, useEffect, useContext } from "react";
import { toImage } from "../../services/coreService";
import { ImageContext } from "../../contexts/ImageContext";
import config from "../../config";

export default function Loading() {
  const { image } = useContext(ImageContext);
  const [currentImagePath, setCurrentImagePath] = useState(null);
  const [urlToRevoke, setUrlToRevoke] = useState(null);
  const [imageLoaded, setImageLoaded] = useState(false);

  const steps = [
    {
      stepId: 0,
      stepDescription: "Uploading images",
      handler: async () => {
        const url = URL.createObjectURL(image);
        if(!config.isProduction){
            console.log(url)
        }
        setCurrentImagePath(url);
        setUrlToRevoke(url);
        setImageLoaded(false);
        await new Promise((resolve) => setTimeout(resolve, 2500));
      },
    },
    {
      stepId: 1,
      stepDescription: "Reprocessing images",
      handler: async () => {
        if(!config.isProduction){
            console.log(image)
        }
        const imgPath = await toImage(image);
        setCurrentImagePath(imgPath);
        setUrlToRevoke(imgPath);
        setImageLoaded(false);
        console.log(imgPath);
      },
    },
    {
      stepId: 2,
      stepDescription: "Extracting texts",
      handler: async () => {
        await new Promise((resolve) => {
          setTimeout(() => {
            console.log("Extracting texts");
            resolve();
          }, 1500);
        });
      },
    },
  ];

  const [currentStep, setCurrentStep] = useState(steps[0]);
  const [isComplete, setIsComplete] = useState(false);

  useEffect(() => {
    async function runSteps() {
      for (const step of steps) {
        setCurrentStep(step);
        await step.handler();
      }
      setIsComplete(true);
    }

    runSteps();

    return () => {
      if (urlToRevoke) {
        URL.revokeObjectURL(urlToRevoke);
      }
    };
  }, []);

  return (
    <div className="process-container">
      <p>{currentStep.stepDescription}</p>

      <div className="loading-line">
        <div
          className="loader-bar"
          style={{
            width: `${((currentStep.stepId + (isComplete ? 1 : 0)) / steps.length) * 100}%`,
          }}
        />
      </div>

      <div className="steps">
        {steps.map((step) => (
          <div
            key={step.stepId}
            className={`dot ${
              isComplete || currentStep.stepId > step.stepId
                ? "done"
                : currentStep.stepId === step.stepId
                ? "active"
                : ""
            }`}
          />
        ))}
      </div>

      <p>
        Processing step {Math.min(currentStep.stepId + 1, steps.length)} of {steps.length}...
      </p>

      {/* Image container with fixed size and loading bone */}
      <div
        style={{
          width: "200px",
          height: "200px",
          marginTop: "20px",
          borderRadius: "8px",
          overflow: "hidden",
          backgroundColor: "#f0f0f0",
          position: "relative",
        }}
      >
        {!imageLoaded && (
          <div
            className="loading-bone"
            style={{
              width: "100%",
              height: "100%",
              background: "linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%)",
              backgroundSize: "200% 100%",
              animation: "skeleton-loading 1.2s infinite",
              position: "absolute",
              top: 0,
              left: 0,
            }}
          />
        )}

        {currentImagePath && (
          <img
            src={currentImagePath}
            alt="Processed"
            onLoad={() => setImageLoaded(true)}
            style={{
              width: "100%",
              height: "100%",
              objectFit: "cover",
              display: imageLoaded ? "block" : "none",
            }}
          />
        )}
      </div>
    </div>
  );
}
