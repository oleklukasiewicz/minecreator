<script lang="ts">
	import Button from "../Button/Button.svelte";
	import CloseIcon from "$icons/close.svg?raw";

	interface Props {
		value?: number;
		clearable?: boolean;
		placeholder?: string;
		min?: number;
		max?: number;
		step?: number | "any";
		integer?: boolean;
		disabled?: boolean;
		oninput?: (value: number | undefined) => void;
	}

	let {
		value = $bindable<number | undefined>(undefined),
		clearable = false,
		placeholder = "",
		min = Number.NEGATIVE_INFINITY,
		max = Number.POSITIVE_INFINITY,
		step = "any",
		integer = false,
		disabled = false,
		oninput,
	}: Props = $props();

	let inputValue = $state(value == null ? "" : String(value));
	let isFocused = $state(false);

	const clamp = (num: number) => Math.min(max, Math.max(min, num));

	const parseValue = (raw: string): number | undefined => {
		const trimmed = raw.trim();
		if (trimmed === "") return undefined;

		const pattern = integer ? /^-?\d+$/ : /^-?(\d+|\d+\.\d+|\.\d+)$/;
		if (!pattern.test(trimmed)) return undefined;

		const parsed = Number(trimmed);
		if (!Number.isFinite(parsed)) return undefined;
		return integer ? Math.trunc(parsed) : parsed;
	};

	const commit = (raw: string) => {
		const parsed = parseValue(raw);

		if (parsed == null) {
			if (raw.trim() === "") {
				value = undefined;
				oninput?.(undefined);
			}
			return;
		}

		value = clamp(parsed);
		oninput?.(value);
	};

	const handleInput = (event: Event) => {
		inputValue = (event.target as HTMLInputElement).value;
		commit(inputValue);
	};

	const handleFocus = () => {
		isFocused = true;
	};

	const handleBlur = () => {
		isFocused = false;
		inputValue = value == null ? "" : String(value);
	};

	const clear = () => {
		value = undefined;
		inputValue = "";
		oninput?.(undefined);
	};

	$effect(() => {
		const normalized = value == null ? "" : String(value);
		if (!isFocused && inputValue !== normalized) inputValue = normalized;
	});
</script>

<div class="text-box number-input" class:disabled>
	<!-- svelte-ignore event_directive_deprecated -->
	<input
		bind:value={inputValue}
		type="text"
		inputmode={integer ? "numeric" : "decimal"}
		{placeholder}
		{disabled}
		oninput={handleInput}
		onfocus={handleFocus}
		onblur={handleBlur}
		step={step}
	/>
	{#if clearable && inputValue.length > 0}
		<Button
			onlyIcon
			style="height: 32px;border-left:2px solid var(--color-theme-D6);"
			icon={CloseIcon}
			type="secondary"
			iconSize="auto"
			noBorder
			onclick={clear}
		/>
	{/if}
</div>

<style lang="scss">
	@use "../TextBox/TextBox.scss";

	.number-input {
		&.disabled {
			opacity: 0.7;
			pointer-events: none;
		}
	}
</style>
