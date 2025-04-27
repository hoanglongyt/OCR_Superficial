export default function StepIndicator({ steps, currentStep, isComplete }) {
  if (!currentStep) return null; // ← Prevents the crash

  return (
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
  );
}
