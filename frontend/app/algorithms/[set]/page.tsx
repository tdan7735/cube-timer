import { notFound } from "next/navigation";
import { AlgorithmBrowser } from "../../../components/AlgorithmBrowser";

export default async function AlgorithmSetPage({ params }: {
  params: Promise<{ set: string }>;
}) {
  const { set } = await params;
  if (set !== "oll" && set !== "pll") notFound();
  return <AlgorithmBrowser key={set} name={set.toUpperCase()} />;
}
