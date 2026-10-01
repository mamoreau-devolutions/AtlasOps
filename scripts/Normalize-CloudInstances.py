#!/usr/bin/env python3

import argparse
import json
from pathlib import Path


def iter_json_array(path: Path):
    decoder = json.JSONDecoder()
    buffer = ""
    position = 0
    started = False

    with path.open("r", encoding="utf-8") as source:
        while True:
            if position >= len(buffer) - 1:
                chunk = source.read(1024 * 1024)
                if not chunk:
                    return
                buffer = buffer[position:] + chunk
                position = 0

            while position < len(buffer) and buffer[position].isspace():
                position += 1

            if not started:
                if buffer[position] != "[":
                    raise ValueError(f"{path} is not a JSON array.")
                position += 1
                started = True
                continue

            while position < len(buffer) and (buffer[position].isspace() or buffer[position] == ","):
                position += 1

            if position < len(buffer) and buffer[position] == "]":
                return

            while True:
                try:
                    value, end = decoder.raw_decode(buffer, position)
                    position = end
                    yield value
                    break
                except json.JSONDecodeError:
                    chunk = source.read(1024 * 1024)
                    if not chunk:
                        raise
                    buffer = buffer[position:] + chunk
                    position = 0


def number(value, default=0.0):
    try:
        return float(value)
    except (TypeError, ValueError):
        return default


def first_architecture(item):
    value = item.get("arch", [])
    if isinstance(value, list):
        return ", ".join(str(entry) for entry in value)
    return str(value or "")


def storage_text(item):
    value = item.get("storage", "")
    if isinstance(value, list):
        return "; ".join(
            " ".join(f"{key}={part}" for key, part in entry.items())
            if isinstance(entry, dict)
            else str(entry)
            for entry in value
        )
    return str(value or "")


def linux_price(provider, pricing):
    if not isinstance(pricing, dict):
        return 0.0

    linux = pricing.get("linux", {})
    if not isinstance(linux, dict):
        return 0.0

    value = linux.get("ondemand")
    if value is None and provider == "Azure":
        value = linux.get("basic")
    return number(value)


def normalize(provider, path, destination):
    count = 0
    with destination.open("a", encoding="utf-8", newline="\n") as output:
        for item in iter_json_array(path):
            sku = str(item.get("instance_type") or item.get("pretty_name") or "")
            if not sku:
                continue

            vcpu = number(item.get("vCPU", item.get("vcpu", 0)))
            memory = number(item.get("memory", 0))
            gpu = number(item.get("GPU", 0))
            pricing = item.get("pricing", {})
            if not isinstance(pricing, dict):
                continue

            for region, region_pricing in sorted(pricing.items()):
                hourly = linux_price(provider, region_pricing)
                if hourly <= 0:
                    continue

                record = {
                    "provider": provider,
                    "sku": sku,
                    "name": str(item.get("pretty_name") or sku),
                    "family": str(item.get("family") or item.get("category") or ""),
                    "region": region,
                    "vCpu": vcpu,
                    "memoryGiB": memory,
                    "gpuCount": gpu,
                    "architecture": first_architecture(item),
                    "storage": storage_text(item),
                    "hourlyUsd": hourly,
                    "monthlyUsd": round(hourly * 730.0, 4),
                }
                output.write(json.dumps(record, separators=(",", ":"), ensure_ascii=True))
                output.write("\n")
                count += 1
    return count


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--aws", required=True, type=Path)
    parser.add_argument("--azure", required=True, type=Path)
    parser.add_argument("--gcp", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    arguments = parser.parse_args()

    arguments.output.parent.mkdir(parents=True, exist_ok=True)
    arguments.output.unlink(missing_ok=True)

    counts = {
        "AWS": normalize("AWS", arguments.aws, arguments.output),
        "Azure": normalize("Azure", arguments.azure, arguments.output),
        "GCP": normalize("GCP", arguments.gcp, arguments.output),
    }
    print(json.dumps(counts, sort_keys=True))


if __name__ == "__main__":
    main()
