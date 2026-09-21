import { notFound } from "next/navigation";
import { AlgorithmSetTabs } from "../../../components/AlgorithmSetTabs";

export default async function AlgorithmSetPage({ params }: {
  params: Promise<{ set: string }>;
}) {
  const { set } = await params;
  if (set !== "oll" && set !== "pll") notFound();
  return <AlgorithmSetTabs key={set} name={set.toUpperCase()} />;
}
